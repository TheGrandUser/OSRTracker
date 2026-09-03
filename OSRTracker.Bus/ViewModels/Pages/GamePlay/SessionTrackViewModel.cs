using CommunityToolkit.Mvvm.ComponentModel;
using OSRTracker.Data;
using OSRTracker.Data.Contracts.Services;
using OSRTracker.Models;
using OSRTracker.ViewModels.Pages.GamePlay.Data;

namespace OSRTracker.ViewModels.Pages.GamePlay;

public partial class SessionTrackViewModel(SessionTrackData data, IAppDbContextFactory dbContextFactory) : UpdateableElementViewModel(dbContextFactory)
{
   private string trackName = data.Name;
   public string TrackName { get => trackName; set => SetUpdatableProperty(ref trackName, value); }

   [ObservableProperty]
   public partial SessionTrackId SessionTrackId { get; set; } = data.Id;

   private string groupDescription = data.GroupDescription ?? string.Empty;
   public string GroupDescription { get => groupDescription; set => SetUpdatableProperty(ref groupDescription, value); }

   protected override void UpdateImpl(AppDbContext dbContext)
   {
      var sessionTrack = dbContext.SessionTracks.Find(SessionTrackId);

      if (sessionTrack is null)
      {
         return;
      }

      sessionTrack.Name = TrackName;
      sessionTrack.GroupDescription = GroupDescription;
   }
}
