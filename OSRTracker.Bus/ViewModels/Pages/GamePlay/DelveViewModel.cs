using OSRTracker.Data;
using OSRTracker.Data.Contracts.Services;
using OSRTracker.Models;
using OSRTracker.ViewModels.Pages.GamePlay.Data;

namespace OSRTracker.ViewModels.Pages.GamePlay;

public partial class DelveViewModel(DelveData data, IAppDbContextFactory dbContextFactory) : UpdateableElementViewModel(dbContextFactory)
{
   private string locationDescription = data.LocationDescription;
   private string locationName = data.LocationName;
   public DelveId DelveId { get; } = data.DelveId;
   public string LocationDescription { get => locationDescription; set => SetUpdatableProperty(ref locationDescription, value); }
   public string LocationName { get => locationName; set => SetUpdatableProperty(ref locationName, value); }
   protected override void UpdateImpl(AppDbContext dbContext)
   {
      var delve = dbContext.Delves.Find(DelveId);
      if (delve is null)
      {
         return;
      }
      delve.LocationDescription = LocationDescription;
      delve.LocationName = LocationName;
   }
}
