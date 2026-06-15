using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace OSRTracker.Views.TemplateSelectors;

public class ItemOrNothingTemplateSelector : DataTemplateSelector
{
   public DataTemplate ItemTemplate { get; set; } = default!;
   public DataTemplate NothingTemplate { get; set; } = default!;

   protected override DataTemplate SelectTemplateCore(object item) => item != null ? ItemTemplate : NothingTemplate;

   protected override DataTemplate SelectTemplateCore(object item, DependencyObject container) => SelectTemplateCore(item);
}
