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
using OSRTracker.Services;
using OSRTracker.ViewModels.Pages.GamePlay;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace OSRTracker.Views.Pages.GamePlay;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ApplySessionXPDialog : ContentDialog, IDialog<ApplySessionXPDialog, ApplySessionXPDialogViewModel>
{
   private readonly ApplySessionXPDialogViewModel ViewModel;

   public ApplySessionXPDialog(ApplySessionXPDialogViewModel vm)
   {
      ViewModel = vm;
      DataContext = vm;

      InitializeComponent();
   }

   public static ApplySessionXPDialog Create(ApplySessionXPDialogViewModel vm) => new(vm);
}
