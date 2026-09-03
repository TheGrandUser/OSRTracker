using OSRTracker.Data;
using OSRTracker.Data.Contracts.Services;
using OSRTracker.Models;
using OSRTracker.ViewModels.Pages.GamePlay.Data;

namespace OSRTracker.ViewModels.Pages.GamePlay;

public partial class SessionDelveViewModel(SessionDelveData data, IAppDbContextFactory dbContextFactory) : UpdateableElementViewModel(dbContextFactory)
{
   private string notes = data.Notes;
   public SessionDelveId SessionDelveId { get; } = data.SessionDelveId;
   public string Notes { get => notes; set => SetUpdatableProperty(ref notes, value); }
   protected override void UpdateImpl(AppDbContext dbContext)
   {
      var sessionDelve = dbContext.SessionDelves.Find(SessionDelveId);
      if (sessionDelve is null)
      {
         return;
      }
      sessionDelve.Notes = Notes;
   }
}