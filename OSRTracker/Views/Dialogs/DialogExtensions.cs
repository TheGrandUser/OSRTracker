using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage.Pickers;
using OSRTracker.Contracts.Messages;
using OSRTracker.Contracts.Services;

namespace OSRTracker.Views.Dialogs;

static class DialogExtensions
{
   extension(FrameworkElement element)
   {
      public async Task<string?> ShowInputTextDialogAsync(
        string title,
        string message = "",
        string defaultText = "",
        string okText = "OK",
        string cancelText = "Cancel")
      {
         var inputBox = new TextBox
         {
            Text = defaultText,
            AcceptsReturn = false,
            Height = 32,
            Margin = new Thickness(0, 8, 0, 0)
         };

         var dialog = new ContentDialog
         {
            Title = title,
            Content = new StackPanel
            {
               Children =
                {
                    new TextBlock { Text = message },
                    inputBox
                }
            },
            PrimaryButtonText = okText,
            CloseButtonText = cancelText,
            XamlRoot = element.XamlRoot,
            DefaultButton = ContentDialogButton.Primary
         };

         var result = await dialog.ShowAsync();

         return result == ContentDialogResult.Primary ? inputBox.Text?.Trim() : null;
      }

      public async Task<SelectRpgResponse> ShowSelectRpgSystemDialogAsync(IRpgSystemFileService rpgSystemFileService)
      {
         var dialog = new ContentDialog()
         {
            XamlRoot = element.XamlRoot,
            PrimaryButtonText = "Blank System",
            SecondaryButtonText = "Select System",
            CloseButtonText = "Cancel",
         };
         var result = await dialog.ShowAsync();
         if (result == ContentDialogResult.Primary)
         {
            return new SelectRpgResponse.BlankSystem();
         }
         else if (result == ContentDialogResult.Secondary)
         {
            FileOpenPicker picker = new FileOpenPicker(App.MainWindow.AppWindow.Id);
            picker.FileTypeChoices.Add("JSON File", [".json"]);
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
               var rpgSystem = await rpgSystemFileService.ImportAsync(file.Path);

               return new SelectRpgResponse.Success(rpgSystem);
            }
            else
            {
               return new SelectRpgResponse.Cancelled();
            }
         }
         else
         {
            return new SelectRpgResponse.Cancelled();
         }
      }

      
   }

}
