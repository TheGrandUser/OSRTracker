using OSRTracker.Data;
using OSRTracker.Data.Contracts.Services;
using OSRTracker.Models;

namespace OSRTracker.ViewModels.Pages.GamePlay;

public partial class MonsterViewModel : UpdateableElementViewModel
{
   private string name;

   public DelveId? DelveId { get; }

   private int quantity;
   private int xpValue;
   private string notes;

   public MonsterViewModel(IAppDbContextFactory dbContextFactory, DelveId? delveId)
      : base(dbContextFactory)
   {
      name = string.Empty;
      DelveId = delveId;
      quantity = 0;
      xpValue = 0;
      notes = "";
   }

   public MonsterViewModel(MonsterEntry me, IAppDbContextFactory dbContextFactory)
      : base(dbContextFactory)
   {
      Id = me.Id;

      name = me.Name;
      DelveId = me.DelveId;
      quantity = me.Quantity;
      xpValue = me.XPValue;
      notes = me.Notes ?? "";
   }

   public MonsterEntryId Id { get; }
   public string Name { get => name; set => SetUpdatableProperty(ref name, value); }
   public int Quantity { get => quantity; set => SetUpdatableProperty(ref quantity, value); }
   public int XPValue { get => xpValue; set => SetUpdatableProperty(ref xpValue, value); }
   public string Notes { get => notes; set => SetUpdatableProperty(ref notes, value); }

   protected override void UpdateImpl(AppDbContext dbContext)
   {
      var entity = dbContext.MonsterEntries.Find(Id);

      if (entity is null)
      {
         return;
      }

      entity.Name = name;
      entity.Quantity = quantity;
      entity.XPValue = xpValue;
      entity.Notes = notes;
   }
}
