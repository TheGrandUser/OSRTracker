using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI.Collections;
using Microsoft.Extensions.DependencyInjection;
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
   private readonly IAppInfoService appInfoService;
   private readonly IDialogService dialogService;
   private readonly ITimeSource timeSource;

   public AdvancedCollectionView CampaignsView { get; }

   private readonly ObservableCollection<RecentFile> recentCampaigns = [];


   public WelcomeStateViewModel(
      List<RecentFile> recentFiles,
      IAppStateService appStateService,
      ILocalSettingsService localSettingsService,
      ILogger<WelcomeStateViewModel> logger,
      IAppInfoService appInfoService,
      IDialogService dialogService,
      ITimeSource timeSource)
   {
      this.appStateService = appStateService;
      this.localSettingsService = localSettingsService;
      this.logger = logger;
      this.appInfoService = appInfoService;
      this.dialogService = dialogService;
      this.timeSource = timeSource;
      CampaignsView = new AdvancedCollectionView(recentCampaigns);
      CampaignsView.SortDescriptions.Add(new SortDescription(nameof(RecentFile.LastAccessed), SortDirection.Ascending));


      foreach (var recentCampaign in recentFiles)
      {
         recentCampaigns.Add(recentCampaign);
      }
   }

   public static async Task<WelcomeStateViewModel> CreateAsync(IServiceProvider services)
   {
      var logger = services.GetRequiredService<ILogger<WelcomeStateViewModel>>();
      var localSettingsService = services.GetRequiredService<ILocalSettingsService>();
      var recentFiles = await LoadMRU(localSettingsService);

      return new WelcomeStateViewModel(
         recentFiles,
         services.GetRequiredService<IAppStateService>(),
         localSettingsService,
         logger,
         services.GetRequiredService<IAppInfoService>(),
         services.GetRequiredService<IDialogService>(),
         services.GetRequiredService<ITimeSource>());
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

         return recentFiles.DistinctBy(rf => rf.Path).ToList();
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
            recentFile.Version = appInfoService.GetAppVersion();
            recentFile.SystemName = campaign.SystemName;
            recentFile.LastAccessed = timeSource.GetUtcNow();

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
         var pickResult = await dialogService.PickSaveFileAsync([("Campaign File", [".cdb"])]);

         if (string.IsNullOrEmpty(pickResult?.Path))
         {
            return;
         }

         using var busy = SendBusy();

         
         var name = await dialogService.GetInputAsync("Campaign Name", "Enter a name for your campaign.", Path.GetFileNameWithoutExtension(pickResult.Path));

         if (string.IsNullOrEmpty(name)) { return; }


         var systemResponse = await dialogService.ShowSelectRpgSystemDialogAsync();

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
               LastAccessed = timeSource.GetUtcNow(),
               CampaignName = name,
               SystemName = rpgSystem?.SystemName,
               Path = file.Path,
               Token = token,
               Version = appInfoService.GetAppVersion(),
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
         var pickResult = await dialogService.PickSingleFileAsync([("Campaign File", [".cdb"])]);

         if (string.IsNullOrEmpty(pickResult?.Path))
         {
            return;
         }

         var existingRecent = recentCampaigns.FirstOrDefault(f => f.Path == pickResult.Path);
         if (existingRecent is not null)
         {
            existingRecent.LastAccessed = timeSource.GetUtcNow();

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
               LastAccessed = timeSource.GetUtcNow(),
               CampaignName = campaign.Name,
               Path = file.Path,
               Token = token,
               SystemName = campaign.SystemName,
               Version = appInfoService.GetAppVersion(),
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
         var file = await dialogService.PickSingleFileAsync([("Campaign File", [".cdb"])]);

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
