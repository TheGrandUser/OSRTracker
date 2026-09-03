using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage.Pickers;
using OSRTracker.Contracts.Messages;
using OSRTracker.Contracts.Services;
using OSRTracker.ViewModels;

namespace OSRTracker.Services;

internal class DialogService : IDialogService
{
   private readonly IServiceProvider serviceProvider;
   private readonly IViewRegistry viewRegistry;
   private readonly IRpgSystemFileService rpgSystemFileService;

   public DialogService(IServiceProvider serviceProvider, IViewRegistry viewRegistry, IRpgSystemFileService rpgSystemFileService)
   {
      this.serviceProvider = serviceProvider;
      this.viewRegistry = viewRegistry;
      this.rpgSystemFileService = rpgSystemFileService;
   }

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

   public async Task<string?> GetInputAsync(
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
         XamlRoot = App.MainWindow.Content.XamlRoot,
         DefaultButton = ContentDialogButton.Primary
      };

      var result = await dialog.ShowAsync();

      return result == ContentDialogResult.Primary ? inputBox.Text?.Trim() : null;
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

   public async Task<ContentDialogResult> ShowDialogAsync<TViewModel>(TViewModel vm) where TViewModel : DialogViewModel
   {
      var dialog = viewRegistry.CreateDialog(vm);

      dialog.XamlRoot = App.MainWindow.Content.XamlRoot;

      var result = await dialog.ShowAsync();

      return result;
   }



   public async Task<SelectRpgResponse> ShowSelectRpgSystemDialogAsync()
   {
      var dialog = new ContentDialog()
      {
         XamlRoot = App.MainWindow.Content.XamlRoot,
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
         var picker = new FileOpenPicker(App.MainWindow.AppWindow.Id);
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
