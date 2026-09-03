using OSRTracker.Data;
using OSRTracker.Data.Contracts.Services;
using OSRTracker.Models;
using OSRTracker.ViewModels.Pages.GamePlay.Data;

namespace OSRTracker.ViewModels.Pages.GamePlay;

public partial class SessionViewModel(SessionData data, IAppDbContextFactory dbContextFactory) : UpdateableElementViewModel(dbContextFactory)
{
   private string sessionNumber = data.SessionNumber;
   private string sessionTitle = data.SessionTitle;
   private string sessionNotes = data.SessionNotes;

   public SessionId SessionId { get; } = data.SessionId;

   public string SessionNumber { get => sessionNumber; set => SetUpdatableProperty(ref sessionNumber, value); }

   public string SessionTitle { get => sessionTitle; set => SetUpdatableProperty(ref sessionTitle, value); }

   public string SessionNotes { get => sessionNotes; set => SetUpdatableProperty(ref sessionNotes, value); }


   protected override void UpdateImpl(AppDbContext dbContext)
   {
      var session = dbContext.Sessions.Find(SessionId);
      if (session is null)
      {
         return;
      }
      session.SessionNumber = SessionNumber;
      session.Title = SessionTitle;
      session.SessionNotes = SessionNotes;
   }
}
