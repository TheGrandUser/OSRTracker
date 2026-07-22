using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage.Pickers;
using OSRTracker.Contracts.Services;

namespace OSRTracker.Services;

internal class DialogService : IDialogService
{
   public async Task<PickFileResult> PickSingleFileAsync((string, List<string>)[] fileTypeChoices)
   {
      var picker = new FileOpenPicker(App.MainWindow.AppWindow.Id);

      foreach (var (name, ext) in fileTypeChoices)
      {
         picker.FileTypeChoices.Add(name, ext);
      }

      return await picker.PickSingleFileAsync();
   }

   public async Task<PickFileResult> PickSaveFileAsync((string, List<string>)[] fileTypeChoices)
   {
      var picker = new FileSavePicker(App.MainWindow.AppWindow.Id);

      foreach (var (name, ext) in fileTypeChoices)
      {
         picker.FileTypeChoices.Add(name, ext);
      }

      var result = await picker.PickSaveFileAsync();

      return result;
   }

   public async Task<ContentDialogResult> ShowContentDialog(string title, string content, string primaryButtonText = "Ok", string secondaryButtonText = "", string closeButtonText = "")
   {

      var dialog = new Microsoft.UI.Xaml.Controls.ContentDialog
      {
         Title = title,
         Content = content,
         PrimaryButtonText = primaryButtonText,
         SecondaryButtonText = secondaryButtonText,
         CloseButtonText = closeButtonText,
         XamlRoot = App.MainWindow.Content.XamlRoot,
      };

      return await dialog.ShowAsync();
   }
}
