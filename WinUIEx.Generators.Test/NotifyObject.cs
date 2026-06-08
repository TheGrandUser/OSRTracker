using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace WinUIEx.Generators.Test;

public abstract class NotifyObject : INotifyPropertyChanged
{
   public event PropertyChangedEventHandler? PropertyChanged;

   protected bool SetProperty<T>(ref T t, T value, [CallerMemberName] string? propertyName = null)
   {
      if (!Equals(t, value))
      {
         t = value;
         OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
         return true;
      }

      return false;
   }

   protected void OnPropertyChanged(string propertyName)
   {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
   }
   protected void OnPropertyChanged(PropertyChangedEventArgs args)
   {
      PropertyChanged?.Invoke(this, args);
   }
}
