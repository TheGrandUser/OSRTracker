using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI.Collections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage.Pickers;
using OSRTracker.Contracts.Messages;
using OSRTracker.Contracts.Services;
using OSRTracker.Data;
using OSRTracker.Helpers;
using OSRTracker.Models;
using OSRTracker.Services;
using OSRTracker.Views.Dialogs;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Popups;

namespace OSRTracker.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
   static readonly string RecentCampaignsKey = "RecentCampaigns";

   private readonly IAppStateService appStateService;
   private readonly ILocalSettingsService localSettingsService;
   private readonly ILogger<MainViewModel> logger;

   public AdvancedCollectionView CampaignsView { get; }

   private readonly ObservableCollection<RecentFile> recentCampaigns = [];

   [RequiresUnreferencedCode("Needs RecentFile.LastAccessed")]
   public MainViewModel(IAppStateService appStateService, ILocalSettingsService localSettingsService, ILogger<MainViewModel> logger)
   {
      this.appStateService = appStateService;
      this.localSettingsService = localSettingsService;
      this.logger = logger;
      CampaignsView = new AdvancedCollectionView(recentCampaigns);
      CampaignsView.SortDescriptions.Add(new SortDescription(nameof(RecentFile.LastAccessed), SortDirection.Ascending));

      LoadMRU();
   }

   private async void LoadMRU()
   {
      try
      {
         var rfTask = localSettingsService.ReadSettingAsync<List<RecentFile>>(RecentCampaignsKey);

         var i = OSRTracker.Data.CompiledModels.AppDbContextModel.Instance;
         if (i is null)
         {
            logger.LogInformation("AppDbContextModel.Instance is null");
         }

         var recentFiles = await rfTask;

         if (recentFiles is null)
         {
            return;
         }

         foreach (var file in recentFiles)
         {
            recentCampaigns.Add(file);
         }
      }
      catch (Exception ex)
      {
         var dialog = new MessageDialog($"An error has occurred when attempting to get the recent files: {ex.Message}", "Recent files error");

         dialog.Commands.Add(new UICommand("Ok"));

         await dialog.ShowAsync();
      }
   }

   private async Task SaveMRU()
   {
      try
      {
         var items = recentCampaigns.ToList();

         await localSettingsService.SaveSettingAsync<List<RecentFile>>(RecentCampaignsKey, items);
      }
      catch (Exception ex)
      {
         var dialog = new MessageDialog($"An error has occurred when attempting to get the recent files: {ex.Message}", "Recent files error");

         dialog.Commands.Add(new UICommand("Ok"));

         await dialog.ShowAsync();
      }
   }

   public ObservableCollection<RecentFile> RecentCampaigns => recentCampaigns;

   IDisposable SendBusy()
   {
      var message = new AppBusyMessage();

      WeakReferenceMessenger.Default.Send(message);

      return message.HasReceivedResponse ? message.Response : Disposable.Empty;
   }



   [RelayCommand]
   private async Task OpenRecentCampaign(RecentFile recentFile)
   {
      try
      {

         using var busy = SendBusy();

         {
            recentFile.LastAccessed = DateTime.UtcNow;

            await SaveMRU();
         }

         await Task.Delay(20);

         var mru = StorageApplicationPermissions.MostRecentlyUsedList;

         var file = await mru.GetFileAsync(recentFile.Token);

         var campaign = await appStateService.OpenCampaignAsync(file.Path);


      }
      catch (OperationCanceledException) { }
   }

   [RelayCommand]
   private async Task CreateCampaign()
   {
      try
      {
         var picker = new FileSavePicker(App.MainWindow.AppWindow.Id);

         picker.FileTypeChoices.Add("Campaign File", [".cdb"]);

         var pickResult = await picker.PickSaveFileAsync();

         if (string.IsNullOrEmpty(pickResult?.Path))
         {
            return;
         }

         using var busy = SendBusy();

         var inputRequest = new InputTextRequest("Campaign Name", "Enter a name for your campaign.", Path.GetFileNameWithoutExtension(pickResult.Path));

         var name = await WeakReferenceMessenger.Default.Send(inputRequest);

         if (string.IsNullOrEmpty(name)) { return; }


         var systemRequest = new SelectRpgSystemRequest();

         var systemResponse = await WeakReferenceMessenger.Default.Send(systemRequest);

         if (systemResponse is SelectRpgResponse.Cancelled)
         {
            return;
         }


         var rpgSystem = systemResponse is SelectRpgResponse.Success success ? success.RpgSystem : null;


         await appStateService.CreateCampaignAsync(pickResult.Path, name, rpgSystem);

         {
            var mru = StorageApplicationPermissions.MostRecentlyUsedList;

            var file = await StorageFile.GetFileFromPathAsync(pickResult.Path);

            var token = mru.Add(file, "Campaign");

            var faToken = StorageApplicationPermissions.FutureAccessList.Add(file);

            var recentCampaign = new RecentFile()
            {
               FutureAccessToken = faToken,
               LastAccessed = DateTime.UtcNow,
               CampaignName = name,
               Path = file.Path,
               Token = token,
            };

            recentCampaigns.Add(recentCampaign);

            await SaveMRU();
         }
      }
      catch (OperationCanceledException) { }
   }

   [RelayCommand]
   private async Task OpenCampaign()
   {
      try
      {
         var picker = new FileOpenPicker(App.MainWindow.AppWindow.Id)
         {
            ViewMode = PickerViewMode.Thumbnail
         };

         picker.FileTypeChoices.Add("Campaign File", [".cdb"]);

         var pickResult = await picker.PickSingleFileAsync();

         if (string.IsNullOrEmpty(pickResult?.Path))
         {
            return;
         }

         var existingRecent = recentCampaigns.FirstOrDefault(f => f.Path == pickResult.Path);
         if (existingRecent is not null)
         {
            existingRecent.LastAccessed = DateTime.UtcNow;

            await SaveMRU();
         }

         using var busy = SendBusy();

         await Task.Delay(5);

         var campaign = await appStateService.OpenCampaignAsync(pickResult.Path);

         if (existingRecent is null)
         {
            var mru = StorageApplicationPermissions.MostRecentlyUsedList;

            var file = await StorageFile.GetFileFromPathAsync(pickResult.Path);

            var token = mru.Add(file, "Campaign");

            var faToken = StorageApplicationPermissions.FutureAccessList.Add(file);

            var recentCampaign = new RecentFile()
            {
               FutureAccessToken = faToken,
               LastAccessed = DateTime.UtcNow,
               CampaignName = campaign.Name,
               Path = file.Path,
               Token = token,
            };

            recentCampaigns.Add(recentCampaign);

            await SaveMRU();
         }

      }
      catch (OperationCanceledException) { }
   }

   [RelayCommand]
   private async Task Rollback()
   {
      try
      {
         var picker = new FileOpenPicker(App.MainWindow.AppWindow.Id)
         {
            ViewMode = PickerViewMode.Thumbnail
         };

         picker.FileTypeChoices.Add("Campaign File", [".cdb"]);

         var file = await picker.PickSingleFileAsync();

         if (string.IsNullOrEmpty(file?.Path))
         {
            return;
         }

         using var busy = SendBusy();

         await Task.Delay(5);

         await appStateService.RollbackCampaignAsync(file.Path);
      }
      catch (OperationCanceledException) { }
   }

   [RelayCommand]
   private async Task TestBusy()
   {
      using var busy = SendBusy();

      await Task.Delay(TimeSpan.FromSeconds(5));
   }
}

public class RecentFile
{
   public required string CampaignName { get; set; }

   public required string Path { get; set; }
   public required string Token { get; set; }
   public required string FutureAccessToken { get; set; }
   public required DateTime LastAccessed { get; set; }
}


