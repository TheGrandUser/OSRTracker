using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace OSRTracker.Views.Converters;

internal partial class BoolToBrushConverter : DependencyObject, IValueConverter
{
   //public static readonly DependencyProperty TrueBrushProperty = DependencyProperty.Register("TrueBrush", typeof(Brush), typeof(BoolToColorConverter));

   public Brush TrueBrush { get; set; } = new SolidColorBrush(Colors.White);
   public Brush FalseBrush { get; set; } = new SolidColorBrush(Colors.Transparent);

   public object Convert(object value, Type targetType, object parameter, string language)
   {
      if (value is true)
      {
         return TrueBrush;
      }
      else
      {
         return FalseBrush;
      }
   }

   public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
