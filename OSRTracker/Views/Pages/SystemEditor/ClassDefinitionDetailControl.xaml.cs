using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OSRTracker.ViewModels;
using OSRTracker.ViewModels.Pages.SystemEditor;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OSRTracker.Views.Pages;

public sealed partial class ClassDefinitionDetailControl : UserControl
{
   public ClassDefinitionDetailControl()
   {
      InitializeComponent();
   }

   public ClassDefinitionViewModel? ViewModel {
      get => (ClassDefinitionViewModel?)GetValue(ViewModelProperty);
      set => SetValue(ViewModelProperty, value);
   }

   public static DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel),
      typeof(ClassDefinitionViewModel), typeof(ClassDefinitionDetailControl), new PropertyMetadata(null, OnViewModelPropertyChanged));

   private static void OnViewModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
   {
      if (d is ClassDefinitionDetailControl control)
      {
         //control.DataContext = e.NewValue;

         if (control.levelsTable is not null)
         {
            //var items = control.ViewModel?.Levels;
            //control.levelsTable.ItemsSource = null;
            //control.levelsTable.ItemsSource = items;
         }
      }
   }

   private void KeyAttributesTextBox_ItemClick(object sender, ItemClickEventArgs e)
   {

   }

   private void KeyAttributesTextBox_TokenItemAdding(TokenizingTextBox sender, TokenItemAddingEventArgs args)
   {
      try
      {
         if (args.Item is AttributeDefinitionViewModel attributeVM)
         {
            args.Item = attributeVM;
         }
         else if (args.Item is string attributeName)
         {
            var newAttribute = ViewModel?.Attributes.FirstOrDefault(a => a.Name.Equals(attributeName));
            if (newAttribute?.Attribute is { } attribute)
            {
               args.Item = attribute;
            }
            else
            {
               args.Cancel = true;
            }
         }
         else
         {
            args.Cancel = true;
         }
      }
      catch (Exception ex)
      {
         System.Diagnostics.Debug.WriteLine($"TokenItemAdding error: {ex}");
         args.Cancel = true;
      }
   }
}
