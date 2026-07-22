using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using OSRTracker.Contracts.Services;
using OSRTracker.Data.Contracts.Services;
using OSRTracker.Models;

namespace OSRTracker.ViewModels.Pages.Main;

public sealed class SessionTrackData
{
   public SessionTrackId Id { get; set; }
   public string Name { get; set; } = string.Empty;
   public string? GroupDescription { get; set; }
   public SessionId? CurrentSessionId { get; set; }
   public string? CurrentSessionNumber { get; set; }
   public string? CurrentSessionTitle { get; set; }


   public bool HasDelve => CurrentDelveId.HasValue;

   public DelveId? CurrentDelveId { get; set; }
   public string? CurrentDelve { get; set; }
}

public enum SessionState
{
   NoSession,
   InactiveSession,
   ActiveSession,
}

public sealed partial class SessionTrackVM(IAppStateService appStateService, IAppDbContextFactory appDbContextFactory) : SessionTrackItem
{
   public SessionTrackId Id { get; set; }
   [ObservableProperty]
   public partial string Name { get; set; } = "";
   [ObservableProperty]
   public partial string? GroupDescription { get; set; }

   public bool HasSession => CurrentSessionId.HasValue;

   public bool IsActiveSession => this.Id == appStateService.ActiveSessionTrackId;

   public SessionId? CurrentSessionId {
      get;
      set {
         if (SetProperty(ref field, value))
         {
            OnPropertyChanged(nameof(HasSession));
            OnPropertyChanged(nameof(IsActiveSession));
         }
      }
   }

   [ObservableProperty]
   public partial string? CurrentSessionNumber { get; set; }
   [ObservableProperty]
   public partial string? CurrentSessionTitle { get; set; }


   public bool HasDelve => CurrentDelveId.HasValue;

   public DelveId? CurrentDelveId {
      get;
      set {
         if (SetProperty(ref field, value))
         {
            OnPropertyChanged(nameof(HasDelve));
         }
      }
   }
   [ObservableProperty]
   public partial string? CurrentDelve { get; set; }

   public ObservableCollection<CharacterSummaryVM> Characters { get; set; } = [];

   internal void OnActiveSessionChanged()
   {
      OnPropertyChanged(nameof(HasSession));
      OnPropertyChanged(nameof(IsActiveSession));
   }

   public void DragOverHandler(object sender, DragEventArgs args)
   {
      if (args.DataView is null)
      {
         return;
      }


      if (args.DataView.Properties.ContainsKey("CharacterVM"))
      {
         var characterVM = (CharacterSummaryVM)args.DataView.Properties["CharacterVM"];

         if (Characters.Any(c => c.Id == characterVM.Id))
         {
            return;
         }

         args.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Link;
      }
   }

   public async void DropHandler(object sender, DragEventArgs args)
   {
      if (args.DataView is null)
      {
         return;
      }

      var characterVM = (CharacterSummaryVM)args.DataView.Properties["CharacterVM"];

      if (Characters.Any(c => c.Id == characterVM.Id))
      {
         return;
      }

      args.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Link;
      args.Handled = true;

      Debug.WriteLine($"Adding character {characterVM.Id} to session track {Id}");

      using var dbContext = await appDbContextFactory.CreateDbContextAsync();

      var stc = new SessionTrackCharacter()
      {
         CharacterId = characterVM.Id,
         SessionTrackId = Id,
      };

      await dbContext.SessionTracksCharacters.AddAsync(stc);
      await dbContext.SaveChangesAsync();

      Characters.Add(characterVM);
   }
}

