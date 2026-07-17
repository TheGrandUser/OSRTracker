using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI.Collections;
using Microsoft.Extensions.Logging;
using Microsoft.Windows.Storage.Pickers;
using OSRTracker.Contracts.Messages;
using OSRTracker.Contracts.Services;
using OSRTracker.Helpers;
using OSRTracker.Services;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Popups;

namespace OSRTracker.ViewModels.Pages.Main;

public partial class WelcomeStateViewModel : MainStateViewModel
{
   private static readonly string RecentCampaignsKey = "RecentCampaigns";

   private readonly IAppStateService appStateService;
   private readonly ILocalSettingsService localSettingsService;
   private readonly ILogger<WelcomeStateViewModel> logger;

   public AdvancedCollectionView CampaignsView { get; }

   private readonly ObservableCollection<RecentFile> recentCampaigns = [];


   public WelcomeStateViewModel(
      List<RecentFile> recentFiles,
      IAppStateService appStateService,
      ILocalSettingsService localSettingsService,
      ILogger<WelcomeStateViewModel> logger)
   {
      this.appStateService = appStateService;
      this.localSettingsService = localSettingsService;
      this.logger = logger;
      CampaignsView = new AdvancedCollectionView(recentCampaigns);
      CampaignsView.SortDescriptions.Add(new SortDescription(nameof(RecentFile.LastAccessed), SortDirection.Ascending));


      foreach (var recentCampaign in recentFiles)
      {
         recentCampaigns.Add(recentCampaign);
      }
   }

   public static async Task<WelcomeStateViewModel> CreateAsync()
   {
      var logger = App.GetService<ILogger<WelcomeStateViewModel>>();
      var localSettingsService = App.GetService<ILocalSettingsService>();
      var recentFiles = await LoadMRU(localSettingsService);

      return new WelcomeStateViewModel(
         recentFiles,
         App.GetService<IAppStateService>(),
         localSettingsService,
         logger
         );
   }

   private static async Task<List<RecentFile>> LoadMRU(ILocalSettingsService localSettingsService)
   {
      try
      {
         var rfTask = localSettingsService.ReadSettingAsync<List<RecentFile>>(RecentCampaignsKey);

         var recentFiles = await rfTask;

         if (recentFiles is null)
         {
            return [];
         }

         return recentFiles;
      }
      catch (Exception ex)
      {
         var dialog = new MessageDialog($"An error has occurred when attempting to get the recent files: {ex.Message}", "Recent files error");

         dialog.Commands.Add(new UICommand("Ok"));

         await dialog.ShowAsync();

         return [];
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


   [RelayCommand]
   private async Task OpenRecentCampaign(RecentFile recentFile)
   {
      try
      {

         using var busy = SendBusy();

         await Task.Delay(20);

         var mru = StorageApplicationPermissions.MostRecentlyUsedList;

         var file = await mru.GetFileAsync(recentFile.Token);

         var campaign = await appStateService.OpenCampaignAsync(file.Path);


         {
            recentFile.Version = App.GetVersion();
            recentFile.SystemName = campaign.SystemName;
            recentFile.LastAccessed = DateTime.UtcNow;

            await SaveMRU();
         }

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

            var recentFile = new RecentFile()
            {
               FutureAccessToken = faToken,
               LastAccessed = DateTime.UtcNow,
               CampaignName = name,
               SystemName = rpgSystem?.SystemName,
               Path = file.Path,
               Token = token,
               Version = App.GetVersion(),
            };

            recentCampaigns.Add(recentFile);

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

            var recentFile = new RecentFile()
            {
               FutureAccessToken = faToken,
               LastAccessed = DateTime.UtcNow,
               CampaignName = campaign.Name,
               Path = file.Path,
               Token = token,
               SystemName = campaign.SystemName,
               Version = App.GetVersion(),
            };


            recentCampaigns.Add(recentFile);

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
   private static async Task TestBusy()
   {
      using var busy = SendBusy();

      await Task.Delay(TimeSpan.FromSeconds(5));
   }
}
