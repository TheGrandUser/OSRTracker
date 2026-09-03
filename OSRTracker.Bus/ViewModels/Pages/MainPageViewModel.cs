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
using OSRTracker.Contracts.ViewModels;
using OSRTracker.Data;
using OSRTracker.Data.Contracts.Services;
using OSRTracker.Helpers;
using OSRTracker.Models;
using OSRTracker.Services;
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

public partial class MainPageViewModel : ObservableRecipient, IRecipient<CampaignOpened>, INavigationAware
{
   private readonly ILocalSettingsService localSettingsService;
   private readonly IServiceProvider serviceProvider;
   private readonly ILogger<MainPageViewModel> logger;

   public MainPageViewModel(
      IAppStateService appStateService,
      ILocalSettingsService localSettingsService,
      IServiceProvider serviceProvider,
      ILogger<MainPageViewModel> logger)
   {
      this.localSettingsService = localSettingsService;
      this.serviceProvider = serviceProvider;
      this.logger = logger;
      State = new EmptyStateViewModel();

      if (string.IsNullOrEmpty(appStateService.CampaignDbPath))
      {
         LoadWelcome();
      }
      else
      {
         LoadCampaign();
      }
   }

   private async void LoadWelcome()
   {
      State = await WelcomeStateViewModel.CreateAsync(serviceProvider);
   }

   private async void LoadCampaign()
   {
      State = await CampaignStateViewModel.CreateAsync(serviceProvider);
   }


   [ObservableProperty]
   public partial MainStateViewModel State { get; set; }

   void IRecipient<CampaignOpened>.Receive(CampaignOpened message)
   {
      LoadCampaign();
   }

   public void OnNavigatedTo(object parameter)
   {
      WeakReferenceMessenger.Default.Register(this);
   }
   public void OnNavigatedFrom()
   {
      WeakReferenceMessenger.Default.UnregisterAll(this);
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


