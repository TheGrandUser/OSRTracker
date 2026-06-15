
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Navigation;
using OSRTracker.Contracts.Messages;
using OSRTracker.Contracts.Services;
using OSRTracker.Data.Contracts.Services;


namespace OSRTracker.ViewModels;

public partial class ShellViewModel : ObservableRecipient, IRecipient<CampaignOpened>, IRecipient<AppBusyMessage>
{
   [ObservableProperty]
   public partial bool IsBackEnabled { get; set; }

   [ObservableProperty]
   public partial object? Selected { get; set; }

   public INavigationService NavigationService {
      get;
   }

   public INavigationViewService NavigationViewService {
      get;
   }

   [ObservableProperty]
   public partial bool IsProjectOpened { get; private set; }

   public ShellViewModel(INavigationService navigationService, INavigationViewService navigationViewService, IAppStateService appStateService)
   {
      NavigationService = navigationService;
      NavigationService.Navigated += OnNavigated;
      NavigationViewService = navigationViewService;

      this.IsProjectOpened = appStateService.Campaign is not null;

      WeakReferenceMessenger.Default.Register<CampaignOpened>(this);
      WeakReferenceMessenger.Default.Register<AppBusyMessage>(this);
   }

   private void OnNavigated(object sender, NavigationEventArgs e)
   {
      IsBackEnabled = NavigationService.CanGoBack;
      var selectedItem = NavigationViewService.GetSelectedItem(e.SourcePageType);
      if (selectedItem != null)
      {
         Selected = selectedItem;
      }
   }

   void IRecipient<CampaignOpened>.Receive(CampaignOpened message)
   {
      IsProjectOpened = true;
   }

   [ObservableProperty]
   public partial bool IsBusy { get; set; }
   void IRecipient<AppBusyMessage>.Receive(AppBusyMessage message)
   {
      Debug.WriteLine("Receive busy");
      IsBusy = true;
      message.Reply(new EndBusy(this));
   }

   class EndBusy(ShellViewModel owner) : IDisposable
   {
      public void Dispose() => owner.IsBusy = false;
   }
}
