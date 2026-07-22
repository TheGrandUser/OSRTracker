using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage.Pickers;

namespace OSRTracker.Contracts.Services;

public interface IDialogService
{
   Task<PickFileResult> PickSaveFileAsync((string, List<string>)[] fileTypeChoices);
   Task<PickFileResult> PickSingleFileAsync((string, List<string>)[] fileTypeChoices);

   Task<ContentDialogResult> ShowContentDialog(string title, string content,
      string primaryButtonText = "Ok", string secondaryButtonText = "", string closeButtonText = "");
}
