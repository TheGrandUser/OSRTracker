using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml;
using OSRTracker.Contracts.Services;
using OSRTracker.ViewModels;
using OSRTracker.ViewModels.Pages.Main;

namespace OSRTracker.Activation;

public class DefaultActivationHandler : ActivationHandler<LaunchActivatedEventArgs>
{
   private readonly INavigationService _navigationService;
   private readonly IAppStateService appStateService;

   public DefaultActivationHandler(INavigationService navigationService, IAppStateService appStateService)
   {
      _navigationService = navigationService;
      this.appStateService = appStateService;
   }

   protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
   {
      // None of the ActivationHandlers has handled the activation.
      return _navigationService.Frame?.Content == null;
   }

   protected async override Task HandleInternalAsync(LaunchActivatedEventArgs args)
   {
      //if (string.IsNullOrEmpty(appStateService.CampaignDbPath))
      //{
      //   // Nav to welcome window
      //}
      //else
      {
         _navigationService.NavigateTo(typeof(MainPageViewModel).FullName!, args.Arguments);
      }

      await Task.CompletedTask;
   }
}
