using OSRTracker.Data;
using OSRTracker.Data.Contracts.Services;
using OSRTracker.Models;

namespace OSRTracker.ViewModels.Pages.GamePlay;

public partial class GeneralXPViewModel : UpdateableElementViewModel
{
   public GeneralXPViewModel(IAppDbContextFactory dbContextFactory)
      : base(dbContextFactory)
   {
      description = "";
   }

   public GeneralXPViewModel(GeneralXPAward data, IAppDbContextFactory dbContextFactory)
      : base(dbContextFactory)
   {
      Id = data.Id;
      DelveId = data.DelveId;

      description = data.Description;
      amount = data.Amount;

   }

   public GeneralXPAwardId Id { get; }
   public DelveId? DelveId { get; }
   public string Description { get => description; set => SetUpdatableProperty(ref description, value); }
   public int Amount { get => amount; set => SetUpdatableProperty(ref amount, value); }

   private string description;
   private int amount;

   protected override void UpdateImpl(AppDbContext dbContext)
   {
      var entity = dbContext.GeneralXPAwards.Find(Id);

      if (entity is null)
      {
         return;
      }

      entity.Amount = amount;
      entity.Description = description;
   }
}
