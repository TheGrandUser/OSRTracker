using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace OSRTracker;

public abstract class RowWrapperBase<T> : INotifyPropertyChanged, IDisposable
   where T : class
{
   private T? _item;
   private bool disposedValue;


   public event PropertyChangedEventHandler? PropertyChanged;
   public event EventHandler? Realized; // raised when placeholder becomes a real item

   public RowWrapperBase(T? initial = null)
   {
      _item = initial;

      if (_item is INotifyPropertyChanged notifyPropertyChanged)
      {
         notifyPropertyChanged.PropertyChanged += NotifyPropertyChanged_PropertyChanged;
      }
   }

   private void NotifyPropertyChanged_PropertyChanged(object? sender, PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

   public bool IsPlaceholder => _item is null;

   protected T Item {
      get {
         if (_item is null) throw new InvalidOperationException("Item is not realized.");
         return _item;
      }
   }

   protected abstract T Create();

   // Force realization and return the real item
   public T EnsureRealItem()
   {
      if (_item is null)
      {
         _item = Create();
         OnPropertyChanged(nameof(IsPlaceholder));
         Realized?.Invoke(this, EventArgs.Empty);

         if (_item is INotifyPropertyChanged notifyPropertyChanged)
         {
            notifyPropertyChanged.PropertyChanged += NotifyPropertyChanged_PropertyChanged;
         }
         AllPropertiesChanged();
      }
      return _item;
   }

   public void SetItem(T item)
   {
      if (_item is INotifyPropertyChanged notifyPropertyChanged)
      {
         notifyPropertyChanged.PropertyChanged -= NotifyPropertyChanged_PropertyChanged;
      }

      _item = item ?? throw new ArgumentNullException(nameof(item));


      if (_item is INotifyPropertyChanged notifyPropertyChanged2)
      {
         notifyPropertyChanged2.PropertyChanged += NotifyPropertyChanged_PropertyChanged;
      }

      AllPropertiesChanged();
   }

   protected abstract void AllPropertiesChanged();

   protected void OnPropertyChanged(string? name) =>
       PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

   protected virtual void Dispose(bool disposing)
   {
      if (!disposedValue)
      {
         if (disposing)
         {

            if (_item is INotifyPropertyChanged notifyPropertyChanged)
            {
               notifyPropertyChanged.PropertyChanged -= NotifyPropertyChanged_PropertyChanged;
            }
         }

         disposedValue = true;
      }
   }

   public void Dispose()
   {
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
   }
}
