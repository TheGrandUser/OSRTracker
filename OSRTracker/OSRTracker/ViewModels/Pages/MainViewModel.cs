using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage.Pickers;
using OSRTracker.Contracts.Messages;
using OSRTracker.Contracts.Services;
using OSRTracker.Data;
using OSRTracker.Models;
using OSRTracker.Services;
using OSRTracker.Views.Dialogs;

namespace OSRTracker.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
   private readonly IAppStateService appStateService;

   public MainViewModel(IAppStateService appStateService)
   {
      this.appStateService = appStateService;
   }

   [ObservableProperty]
   public partial bool IsBusy { get; set; }

   [RelayCommand]
   private async Task CreateCampaign()
   {
      try
      {
         FileSavePicker picker = new FileSavePicker(App.MainWindow.AppWindow.Id);

         picker.FileTypeChoices.Add("Campaign File", [".cdb"]);

         IsBusy = true;

         var file = await picker.PickSaveFileAsync();

         if (string.IsNullOrEmpty(file?.Path))
         {
            return;
         }

         var inputRequest = new InputTextRequest("Campaign Name", "Enter a name for your campaign.", Path.GetFileNameWithoutExtension(file.Path));

         var name = await WeakReferenceMessenger.Default.Send(inputRequest);

         if (string.IsNullOrEmpty(name)) { return; }


         var systemRequest = new SelectRpgSystemRequest();

         var systemResponse = await WeakReferenceMessenger.Default.Send(systemRequest);

         if (systemResponse is SelectRpgResponse.Cancelled)
         {
            return;
         }


         SystemDto? rpgSystem = systemResponse is SelectRpgResponse.Success success ? success.RpgSystem : null;


         await this.appStateService.CreateCampaignAsync(file.Path, name, rpgSystem);

      }
      catch (OperationCanceledException) { }
      finally
      {
         IsBusy = false;
      }
   }

   [RelayCommand]
   private async Task OpenCampaign()
   {
      try
      {
         FileOpenPicker picker = new FileOpenPicker(App.MainWindow.AppWindow.Id)
         {
            ViewMode = PickerViewMode.Thumbnail
         };
         
         picker.FileTypeChoices.Add("Campaign File", [".cdb"]);

         var file = await picker.PickSingleFileAsync();

         if (string.IsNullOrEmpty(file?.Path))
         {
            return;
         }

         IsBusy = true;

         await Task.Delay(5);

         await appStateService.OpenCampaignAsync(file.Path);
      }
      catch (OperationCanceledException) { }
      finally
      {
         IsBusy = false;
      }
   }

   [RelayCommand]
   private async Task Rollback()
   {
      try
      {
         FileOpenPicker picker = new FileOpenPicker(App.MainWindow.AppWindow.Id);

         IsBusy = true;

         picker.ViewMode = PickerViewMode.Thumbnail;
         //picker.SuggestedStartLocation
         picker.FileTypeChoices.Add("Campaign File", [".cdb"]);

         var file = await picker.PickSingleFileAsync();

         if (string.IsNullOrEmpty(file?.Path))
         {
            return;
         }

         await appStateService.RollbackCampaignAsync(file.Path);
      }
      catch (OperationCanceledException) { }
      finally
      {
         IsBusy = false;
      }
   }
}
