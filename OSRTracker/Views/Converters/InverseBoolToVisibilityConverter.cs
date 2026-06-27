using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.WinUI.Converters;
using Microsoft.UI.Xaml;

namespace OSRTracker.Views.Converters;


internal partial class InverseBoolToVisibilityConverter : BoolToObjectConverter
{
   public InverseBoolToVisibilityConverter()
   {
      TrueValue = Visibility.Collapsed;
      FalseValue = Visibility.Visible;
   }
}
