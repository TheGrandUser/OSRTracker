using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace OSRTracker.ViewModels;

public abstract class DialogViewModel : ObservableObject, IDisposable
{
   private bool isPrimaryEnabled;
   private string primaryText, closeText;
   private bool disposedValue;

   protected DialogViewModel()
   {
      
      PrimaryCommand = new AsyncRelayCommand(OnPrimaryExecuted);

      isPrimaryEnabled = true;
      primaryText = "Ok";
      closeText = "Cancel";
   }

   public ICommand PrimaryCommand { get; }

   public bool IsPrimaryEnabled {
      get => isPrimaryEnabled;
      protected set => SetProperty(ref isPrimaryEnabled, value);
   }

   public string PrimaryText {
      get => primaryText;
      protected set => SetProperty(ref primaryText, value);
   }

   public string CloseText {
      get => closeText;
      protected set => SetProperty(ref closeText, value);
   }

   protected abstract Task OnPrimaryExecuted();

   protected virtual void Dispose(bool disposing)
   {
      if (!disposedValue)
      {
         if (disposing)
         {
         }

         disposedValue = true;
      }
   }

   public void Dispose()
   {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
   }
}
