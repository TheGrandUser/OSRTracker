using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OSRTracker.Helpers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OSRTracker;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : WindowEx
{
   private Microsoft.UI.Dispatching.DispatcherQueue dispatcherQueue;

   private UISettings settings;

   public MainWindow()
   {
      InitializeComponent();

      AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/WindowIcon.ico"));
      Content = null;
      Title = "AppDisplayName".GetLocalized();

      // Theme change code picked from https://github.com/microsoft/WinUI-Gallery/pull/1239
      dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
      settings = new UISettings();
      settings.ColorValuesChanged += Settings_ColorValuesChanged; // cannot use FrameworkElement.ActualThemeChanged event
   }

   // this handles updating the caption button colors correctly when indows system theme is changed
   // while the app is open
   private void Settings_ColorValuesChanged(UISettings sender, object args)
   {
      // This calls comes off-thread, hence we will need to dispatch it to current app's thread
      dispatcherQueue.TryEnqueue(() =>
      {
         TitleBarHelper.ApplySystemThemeToCaptionButtons();
      });
   }
}
