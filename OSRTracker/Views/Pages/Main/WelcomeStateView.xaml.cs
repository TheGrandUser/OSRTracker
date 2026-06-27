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
using OSRTracker.ViewModels.Pages.Main;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OSRTracker.Views.Pages.Main;

public sealed partial class WelcomeStateView : UserControl
{
   public DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(WelcomeStateViewModel), typeof(WelcomeStateView),
      new PropertyMetadata(null));

   public WelcomeStateViewModel? ViewModel { get => (WelcomeStateViewModel?)GetValue(ViewModelProperty); set => SetValue(ViewModelProperty, value); }

   public WelcomeStateView()
   {
      InitializeComponent();
   }
}
