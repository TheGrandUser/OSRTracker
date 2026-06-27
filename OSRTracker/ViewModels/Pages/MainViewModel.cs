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
using OSRTracker.Data.Contracts.Services;
using OSRTracker.Helpers;
using OSRTracker.Models;
using OSRTracker.Services;
using OSRTracker.Views.Dialogs;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Popups;

namespace OSRTracker.ViewModels.Pages.Main;

public abstract class MainStateViewModel : ObservableObject {
   protected static IDisposable SendBusy()
   {
      var message = new AppBusyMessage();

      WeakReferenceMessenger.Default.Send(message);

      return message.HasReceivedResponse ? message.Response : Disposable.Empty;
   }
}

public partial class MainViewModel : ObservableRecipient, IRecipient<CampaignOpened>
{
   private readonly IAppStateService appStateService;
   private readonly ILocalSettingsService localSettingsService;
   private readonly ILogger<MainViewModel> logger;
   private readonly IAppDbContextFactory dbContextFactory;

   public MainViewModel(IAppStateService appStateService, ILocalSettingsService localSettingsService, ILogger<MainViewModel> logger,
      IAppDbContextFactory dbContextFactory)
   {
      this.appStateService = appStateService;
      this.localSettingsService = localSettingsService;
      this.logger = logger;
      this.dbContextFactory = dbContextFactory;


      State = new EmptyStateViewModel();

      if (string.IsNullOrEmpty(appStateService.CampaignDbPath))
      {
         LoadWelcome();
      }
      else
      {
         LoadCampaign();
      }

      WeakReferenceMessenger.Default.Register(this);
   }

   private async void LoadWelcome()
   {
      State = await WelcomeStateViewModel.CreateAsync();
   }

   private async void LoadCampaign()
   {
      State = await CampaignStateViewModel.CreateAsync(dbContextFactory, appStateService);
   }


   [ObservableProperty]
   public partial MainStateViewModel State { get; set; }

   private static IDisposable SendBusy()
   {
      var message = new AppBusyMessage();

      WeakReferenceMessenger.Default.Send(message);

      return message.HasReceivedResponse ? message.Response : Disposable.Empty;
   }

   void IRecipient<CampaignOpened>.Receive(CampaignOpened message)
   {
      LoadCampaign();
   }
}

public class RecentFile
{
   public RecentFile()
   {
      
   }

   public required string CampaignName { get; set; }

   public required string Path { get; set; }
   public required string Token { get; set; }
   public required string FutureAccessToken { get; set; }
   public required DateTime LastAccessed { get; set; }
   public string? SystemName { get; set; }
   public required string Version { get; set; }
}


