
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Navigation;
using OSRTracker.Contracts.Messages;
using OSRTracker.Contracts.Services;
using OSRTracker.Data.Contracts.Services;


namespace OSRTracker.ViewModels;

public partial class ShellViewModel : ObservableRecipient, IRecipient<CampaignOpened>
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
}
