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
using OSRTracker.Models;
using OSRTracker.ViewModels.Pages.Main;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OSRTracker.Views.Pages.Main;

public sealed partial class CampaignStateView : UserControl
{
   public DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(CampaignStateViewModel), typeof(CampaignStateView),
      new PropertyMetadata(null));

   public CampaignStateViewModel? ViewModel {
      get {
         var value = GetValue(ViewModelProperty);
         return (CampaignStateViewModel?)value;
      }
      set => SetValue(ViewModelProperty, value);
   }

   public CampaignStateView()
   {
      InitializeComponent();
   }

   private void SessionTrackView_ItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args)
   {

   }

   private void CharactersView_ItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args)
   {

   }

   private async void AddSessionTrack_Click(object sender, RoutedEventArgs e)
   {
      if (ViewModel is null)
      {
         return;
      }

      var dialog = new CreateSessionTrackDialog()
      {
         XamlRoot = XamlRoot
      };

      var result = await dialog.ShowAsync(ContentDialogPlacement.Popup);

      if (result == ContentDialogResult.Primary)
      {
         var sessTrack = new SessionTrack() { Name = dialog.TrackName, GroupDescription = dialog.Description };

         ViewModel.CreateNewSessionTrackCommand.Execute(sessTrack);
      }
   }

   private void CharactersView_DragStarting(UIElement sender, DragStartingEventArgs args)
   {
      if (args.Data is null)
      {
         return;
      }

      if (sender is not ItemContainer itemContainer || itemContainer.DataContext is not CharacterSummaryVM vm)
      {
         return;
      }

      args.Data.Properties.Add("CharacterVM", vm);
      args.Data.RequestedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Link;
   }

   private void sessionTrackContainer_DragOver(object sender, DragEventArgs e)
   {

   }

   private void sessionTrackContainer_Drop(object sender, DragEventArgs e)
   {

   }
}
