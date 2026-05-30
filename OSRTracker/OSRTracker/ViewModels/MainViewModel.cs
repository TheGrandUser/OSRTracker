using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Windows.Storage.Pickers;
using OSRTracker.Contracts.Messages;
using OSRTracker.Contracts.Services;
using OSRTracker.Data;
using OSRTracker.Services;

namespace OSRTracker.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
   private readonly IAppStateService appStateService;

   public MainViewModel(IAppStateService appStateService)
   {
      this.appStateService = appStateService;
   }

   [RelayCommand]
   private async Task CreateCampaign()
   {
      try
      {
         FileSavePicker picker = new FileSavePicker(App.MainWindow.AppWindow.Id);

         picker.FileTypeChoices.Add("Campaign File", [".cdb"]);

         var file = await picker.PickSaveFileAsync();

         if (string.IsNullOrEmpty(file?.Path))
         {
            return;
         }

         await this.appStateService.CreateCampaignAsync(file.Path);

      }
      catch (OperationCanceledException) { }

   }

   [RelayCommand]
   private async Task OpenCampaign()
   {
      try
      {
         FileOpenPicker picker = new FileOpenPicker(App.MainWindow.AppWindow.Id);

         picker.ViewMode = PickerViewMode.Thumbnail;
         //picker.SuggestedStartLocation
         picker.FileTypeChoices.Add("Campaign File", [".cdb"]);

         var file = await picker.PickSingleFileAsync();

         if (string.IsNullOrEmpty(file?.Path))
         {
            return;
         }

         await this.appStateService.OpenCampaignAsync(file.Path);
      }
      catch (OperationCanceledException) { }
   }
}
