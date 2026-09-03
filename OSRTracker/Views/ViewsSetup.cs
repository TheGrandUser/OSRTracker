using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using OSRTracker.Services;
using OSRTracker.ViewModels;
using OSRTracker.ViewModels.Pages.CharacterRoster;
using OSRTracker.ViewModels.Pages.GamePlay;
using OSRTracker.ViewModels.Pages.GamePlay.Data;
using OSRTracker.ViewModels.Pages.Main;
using OSRTracker.ViewModels.Pages.Main.Data;
using OSRTracker.ViewModels.Pages.SystemEditor;
using OSRTracker.Views.Pages;
using OSRTracker.Views.Pages.GamePlay;
using OSRTracker.Views.Pages.Main;

namespace OSRTracker.Views;

internal static class ViewsSetup
{
   extension(IServiceCollection services)
   {
      public IServiceCollection AddViews()
      {

         services.AddSingleton<IViewRegistry, ViewRegistry>(services =>
         {
            var viewRegistry = new ViewRegistry();

            viewRegistry.Register<ApplySessionXPDialogViewModel, ApplySessionXPDialog>();
            viewRegistry.Register<ApplyDelveXPDialogViewModel, ApplyDelveXPDialog>();

            return viewRegistry;
         });

         services.AddTransient<IGamePlayDataRepo, GamePlayDataRepo>();
         services.AddTransient<IMainPageDataRepo, MainPageDataRepo>();

         // Views and ViewModels
         services.AddTransient<ShellPage>();
         services.AddTransient<ShellViewModel>();

         services.AddTransient<MainPageViewModel>();
         services.AddTransient<WelcomeStateViewModel>();
         services.AddTransient<CampaignStateViewModel>();
         services.AddTransient<MainPage>();

         services.AddTransient<SystemEditorPageViewModel>();
         services.AddTransient<SystemEditorPage>();

         services.AddTransient<CharacterRosterPageViewModel>();
         services.AddTransient<CharacterRosterPage>();

         services.AddTransient<GamePlayPageViewModel>();
         services.AddTransient<GamePlayPage>();

         services.AddTransient<ApplySessionXPDialogViewModel>();
         services.AddTransient<ApplySessionXPDialog>();

         return services;
      }
   }
}
