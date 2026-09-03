using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage.Pickers;
using OSRTracker.Contracts.Messages;
using OSRTracker.Models;
using OSRTracker.ViewModels;

namespace OSRTracker.Contracts.Services;

public abstract record SelectRpgResponse
{
   public sealed record Success(SystemDto RpgSystem) : SelectRpgResponse;
   public sealed record Cancelled() : SelectRpgResponse;
   public sealed record BlankSystem() : SelectRpgResponse;
}
public interface IDialogService
{
   Task<PickFileResult> PickSaveFileAsync((string, List<string>)[] fileTypeChoices);
   Task<PickFileResult> PickSingleFileAsync((string, List<string>)[] fileTypeChoices);

   Task<ContentDialogResult> ShowContentDialog(string title, string content,
      string primaryButtonText = "Ok", string secondaryButtonText = "", string closeButtonText = "");

   Task<string?> GetInputAsync(
      string title,
        string message = "",
        string defaultText = "",
        string okText = "OK",
        string cancelText = "Cancel");

   Task<ContentDialogResult> ShowDialogAsync<TViewModel>(TViewModel vm)
       where TViewModel : DialogViewModel;

   Task<SelectRpgResponse> ShowSelectRpgSystemDialogAsync();
}


