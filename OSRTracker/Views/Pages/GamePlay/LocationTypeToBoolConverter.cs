using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml.Data;
using OSRTracker.Models;
using OSRTracker.ViewModels.Pages.GamePlay;

namespace OSRTracker.Views.Pages.GamePlay;

internal partial class LocationTypeToBoolConverter : IValueConverter
{
   public LocationType LocationType { get; set; }

   public object Convert(object value, Type targetType, object parameter, string language)
   {
      if (value is TreasureLocationTypeViewModel treasureLocationTypeViewModel)
      {
         return treasureLocationTypeViewModel.Value == LocationType;
      }
      else if (value is LocationType locationType)
      {
         return locationType == LocationType;
      }

      return false;
   }
   public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
