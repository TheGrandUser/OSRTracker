using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using OSRTracker.Data;
using OSRTracker.Data.Contracts.Services;
using ThrottleDebounce;

namespace OSRTracker.ViewModels;

public abstract class UpdateableElementViewModel : ObservableObject, IDisposable
{
   bool hasPendingUpdate = false;
   private bool disposedValue;
   private readonly AppDbContext appDbContext;

   private readonly RateLimitedAction update;

   protected UpdateableElementViewModel(AppDbContext appDbContext)
   {
      this.appDbContext = appDbContext;

      this.update = Debouncer.Debounce(() => UpdateInternal(), TimeSpan.FromMicroseconds(400));
   }

   protected void SetUpdatableProperty<T>(ref T field, T value, [CallerMemberName]string? propertyName = null)
   {
      if (!EqualityComparer<T>.Default.Equals(field, value))
      {
         Debug.WriteLine($"Updating property {propertyName}");
         field = value;
         OnPropertyChanged(propertyName);
         Update();
      }
   }

   public void Update()
   {
      this.hasPendingUpdate = true;
      update.Invoke();
   }

   public void ForceUpdate(AppDbContext dbContext) {
      if (hasPendingUpdate)
      {
         this.update.Dispose();
         hasPendingUpdate = false;
         UpdateImpl(dbContext);
      }
   }

   private void UpdateInternal()
   {
      this.hasPendingUpdate = false;
      
      UpdateImpl(appDbContext);

      appDbContext.SaveChanges();
   }
   protected abstract void UpdateImpl(AppDbContext dbContext);

   protected virtual void Dispose(bool disposing)
   {
      if (!disposedValue)
      {
         if (disposing)
         {
            this.update.Dispose();
            hasPendingUpdate = false;
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
