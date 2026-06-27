using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using OSRTracker.Activation;
using OSRTracker.Contracts.Services;
using OSRTracker.Core.Contracts.Services;
using OSRTracker.Core.Services;
using OSRTracker.Data.Contracts.Services;
using OSRTracker.Models;
using OSRTracker.Services;
using OSRTracker.ViewModels;
using OSRTracker.ViewModels.Pages;
using OSRTracker.ViewModels.Pages.Main;
using OSRTracker.ViewModels.Pages.SystemEditor;
using OSRTracker.Views;
using OSRTracker.Views.Pages;
using OSRTracker.Views.Pages.Main;
using Windows.ApplicationModel;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OSRTracker;
/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
   // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
   // https://docs.microsoft.com/dotnet/core/extensions/generic-host
   // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
   // https://docs.microsoft.com/dotnet/core/extensions/configuration
   // https://docs.microsoft.com/dotnet/core/extensions/logging
   public IHost Host
   {
      get;
   }

   public static T GetService<T>()
       where T : class
   {
      App cur = (App.Current as App)!;
      if (cur.Host.Services.GetService(typeof(T)) is not T service)
      {
         throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
      }

      return service;
   }

   public static WindowEx MainWindow { get; } = new MainWindow();

   public static UIElement? AppTitlebar
   {
      get; set;
   }

   //private Window? _window;

   /// <summary>
   /// Initializes the singleton application object.  This is the first line of authored code
   /// executed, and as such is the logical equivalent of main() or WinMain().
   /// </summary>
   public App()
   {
      InitializeComponent();

      TypeMappings.AddTypeMappings();
      

      Host = Microsoft.Extensions.Hosting.Host.
      CreateDefaultBuilder().
      UseContentRoot(AppContext.BaseDirectory)
#if DEBUG
      .ConfigureLogging(logging =>
      {
         logging.AddDebug();
      })
#endif
      .ConfigureServices((context, services) =>
      {
         // Default Activation Handler
         services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

         // Other Activation Handlers

         // Services
         services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
         services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
         services.AddTransient<INavigationViewService, NavigationViewService>();

         services.AddSingleton<IActivationService, ActivationService>();
         services.AddSingleton<IPageService, PageService>();
         services.AddSingleton<INavigationService, NavigationService>();


         // Core Services
         services.AddTransient<IRpgSystemFileService, RpgSystemFileService>();
         services.AddSingleton<IFileService, FileService>();

         services.AddSingleton<IAppStateService, AppStateService>();

         services.AddSingleton<IAppDbContextFactory, AppDbContextFactory2>();

         // Views and ViewModels
         services.AddTransient<MainViewModel>();
         services.AddTransient<WelcomeStateViewModel>();
         services.AddTransient<CampaignStateViewModel>();
         services.AddTransient<MainPage>();
         services.AddTransient<ShellPage>();
         services.AddTransient<ShellViewModel>();
         services.AddTransient<SystemEditorViewModel>();
         services.AddTransient<SystemEditorPage>();
         services.AddTransient<CharacterRosterViewModel>();
         services.AddTransient<CharacterRosterPage>();

         // Configuration
         services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
      }).
      Build();

      UnhandledException += App_UnhandledException;
   }

   private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
   {
      // TODO: Log and handle exceptions as appropriate.
      // https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception.
   }

   /// <summary>
   /// Invoked when the application is launched.
   /// </summary>
   /// <param name="args">Details about the launch request and process.</param>
   protected async override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
   {
      base.OnLaunched(args);

      await App.GetService<IActivationService>().ActivateAsync(args);
   }

   internal static string GetVersion()
   {
      var package = Package.Current;
      var packageId = package.Id;
      var version = packageId.Version;

      return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
   }
}
