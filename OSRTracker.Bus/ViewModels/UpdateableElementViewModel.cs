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
   private bool hasPendingUpdate = false;
   private bool disposedValue;
   protected readonly IAppDbContextFactory dbContextFactory;

   private readonly RateLimitedAction update;

   protected UpdateableElementViewModel(IAppDbContextFactory dbContextFactory)
   {
      this.dbContextFactory = dbContextFactory;

      update = Debouncer.Debounce(() => UpdateInternal(), TimeSpan.FromMicroseconds(400));
   }

   protected void SetUpdatableProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
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
      hasPendingUpdate = true;
      update.Invoke();
   }

   public void ForceUpdate(AppDbContext dbContext)
   {
      if (hasPendingUpdate)
      {
         update.Dispose();
         hasPendingUpdate = false;
         UpdateImpl(dbContext);
      }
   }

   private void UpdateInternal()
   {
      hasPendingUpdate = false;

      using var appDbContext = dbContextFactory.CreateDbContext();

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
            update.Dispose();
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
