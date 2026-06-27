using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml;

namespace OSRTracker.Views.StateTriggers;

public class EnumStateTrigger : StateTriggerBase
{
   public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(object), typeof(EnumStateTrigger),
            new PropertyMetadata(null, OnValueChanged));

   public static readonly DependencyProperty ActiveValueProperty =
       DependencyProperty.Register(nameof(ActiveValue), typeof(object), typeof(EnumStateTrigger),
           new PropertyMetadata(null, OnValueChanged));

   public object Value {
      get => GetValue(ValueProperty);
      set => SetValue(ValueProperty, value);
   }

   public object ActiveValue {
      get => GetValue(ActiveValueProperty);
      set => SetValue(ActiveValueProperty, value);
   }

   public EnumStateTrigger()
   {
      
   }

   private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
   {
      var trigger = (EnumStateTrigger)d;
      trigger.UpdateTrigger();
   }

   private void UpdateTrigger()
   {
      SetActive(Value != null && ActiveValue != null && Value.Equals(ActiveValue));
   }
}
