using CommunityToolkit.Mvvm.ComponentModel;
using OSRTracker.Data;
using OSRTracker.Data.Contracts.Services;
using OSRTracker.Models;
using OSRTracker.ViewModels.Pages.GamePlay.Data;

namespace OSRTracker.ViewModels.Pages.GamePlay;

public partial class CharacterViewModel : UpdateableElementViewModel
{
   public CharacterId Id { get; }

   private bool inSession;
   private readonly ClassDefinition? classDef;
   private readonly GamePlayViewModel owner;

   public CharacterViewModel(
      CharacterDto dto,
      ClassDefinition? classDef,
      GamePlayViewModel owner,
      IAppDbContextFactory dbContextFactory) : base(dbContextFactory)
   {
      this.owner = owner;
      Id = dto.Id;
      inSession = dto.InSession;
      this.classDef = classDef;
      Name = dto.Name;
      Level = dto.Level;
      ClassId = dto.ClassId;
      ClassName = dto.ClassName;

      var xpBonus = dto.XPBonus;

      Bonus = $"{xpBonus * 100}%";

      if (classDef is not null)
      {
         var (floorXP, nextXP) = classDef.GetFloorAndNext(Level);

         FloorXP = floorXP;
         NextXP = nextXP;

      }
      else
      {
         FloorXP = 0;
         NextXP = 500;
      }
   }

   public string Name { get; }

   [ObservableProperty]
   public partial int Level { get; set; }

   public ClassDefinitionId? ClassId { get; }

   public string? ClassName { get; }

   [ObservableProperty]
   public partial int XP { get; set; }

   [ObservableProperty]
   public partial int FloorXP { get; set; }

   [ObservableProperty]
   public partial int NextXP { get; set; }

   [ObservableProperty]
   public partial int RemainingXP { get; set; }

   public string Bonus { get; set; }


   public bool InSession { get => inSession; set => SetUpdatableProperty(ref inSession, value); }

   protected override void UpdateImpl(AppDbContext dbContext)
   {
      if (owner.CurrentSession?.SessionId is not SessionId sessionId)
      {
         return;
      }

      var character = dbContext.Characters.Find(Id);
      var session = dbContext.Sessions.Find(sessionId);

      if (character is null || session is null)
      {
         // Error of some sort?

         return;
      }

      if (InSession)
      {
         session.Characters.Remove(character);
      }
      else
      {
         session.Characters.Add(character);
      }
   }
}
