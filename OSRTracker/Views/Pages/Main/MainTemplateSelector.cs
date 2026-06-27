using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OSRTracker.ViewModels.Pages.Main;

namespace OSRTracker.Views.Pages.Main;

internal class MainTemplateSelector : DataTemplateSelector
{
   public DataTemplate WelcomeTemplate { get; set; } = null!;
   public DataTemplate CampaignTemplate { get; set; } = null!;
   public DataTemplate EmptyTemplate { get; set; } = null!;

   protected override DataTemplate SelectTemplateCore(object item)
   {
      return item switch
      {
         CampaignStateViewModel => CampaignTemplate,
         WelcomeStateViewModel => WelcomeTemplate,
         _ => EmptyTemplate,
      };
   }

   protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
   {
      return item switch
      {
         CampaignStateViewModel => CampaignTemplate,
         WelcomeStateViewModel => WelcomeTemplate,
         _ => EmptyTemplate,
      };
   }
}
