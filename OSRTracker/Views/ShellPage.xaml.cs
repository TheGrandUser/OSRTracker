using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OSRTracker.Contracts.Messages;
using OSRTracker.Contracts.Services;
using OSRTracker.Helpers;
using OSRTracker.ViewModels;
using OSRTracker.Views.Dialogs;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OSRTracker.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ShellPage : Page
{
   public ShellViewModel ViewModel {
      get;
   }

   private readonly IRpgSystemFileService rpgSystemFileService;

   public ShellPage(ShellViewModel viewModel, IRpgSystemFileService rpgSystemFileService)
   {
      ViewModel = viewModel;
      this.rpgSystemFileService = rpgSystemFileService;
      InitializeComponent();

      ViewModel.NavigationService.Frame = NavigationFrame;
      ViewModel.NavigationViewService.Initialize(NavigationViewControl);

      // TODO: Set the title bar icon by updating /Assets/WindowIcon.ico.
      // A custom title bar is required for full window theme and Mica support.
      // https://docs.microsoft.com/windows/apps/develop/title-bar?tabs=winui3#full-customization
      App.MainWindow.ExtendsContentIntoTitleBar = true;
      App.MainWindow.SetTitleBar(AppTitleBar);
      App.MainWindow.Activated += MainWindow_Activated;
      AppTitleBarText.Text = "AppDisplayName".GetLocalized();

      WeakReferenceMessenger.Default.Register<InputTextRequest>(this, ShowInputDialog);
   }

   private async void ShowInputDialog(object recipient, InputTextRequest msg)
   {
      var result = await this.ShowInputTextDialogAsync(msg.Title, msg.Message, msg.DefaultText);
      msg.Reply(result);
   }

   private async void ShowSelectRpgSystemDialog(object recipient, SelectRpgSystemRequest msg)
   {
      var result = await this.ShowSelectRpgSystemDialogAsync(this.rpgSystemFileService);
      msg.Reply(result);
   }

   private void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
   {
      TitleBarHelper.UpdateTitleBar(RequestedTheme);

      KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu));
      KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.GoBack));
   }

   private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
   {
      App.AppTitlebar = AppTitleBarText as UIElement;
   }

   private void NavigationViewControl_DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
   {
      AppTitleBar.Margin = new Thickness()
      {
         Left = sender.CompactPaneLength * (sender.DisplayMode == NavigationViewDisplayMode.Minimal ? 2 : 1),
         Top = AppTitleBar.Margin.Top,
         Right = AppTitleBar.Margin.Right,
         Bottom = AppTitleBar.Margin.Bottom
      };
   }

   private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
   {
      var keyboardAccelerator = new KeyboardAccelerator() { Key = key };

      if (modifiers.HasValue)
      {
         keyboardAccelerator.Modifiers = modifiers.Value;
      }

      keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;

      return keyboardAccelerator;
   }

   private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
   {
      var navigationService = App.GetService<INavigationService>();

      var result = navigationService.GoBack();

      args.Handled = result;
   }
}
