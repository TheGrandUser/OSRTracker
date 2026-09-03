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
   private readonly ClassLevels? classDef;
   private readonly GamePlayPageViewModel owner;

   public CharacterViewModel(
      CharacterDto dto,
      ClassLevels? classDef,
      GamePlayPageViewModel owner,
      IAppDbContextFactory dbContextFactory) : base(dbContextFactory)
   {
      this.owner = owner;
      Id = dto.Id;
      inSession = dto.InSession;
      this.classDef = classDef;
      Name = dto.Name;
      Level = dto.Level;
      ClassId = dto.ClassId;

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

   public string? ClassName => classDef?.Name;

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
         var sessionCharacter = session.Characters.FirstOrDefault(sc => sc.CharacterId == character.Id);
         if (sessionCharacter is not null)
         {
            session.Characters.Remove(sessionCharacter);
            dbContext.SessionCharacters.Remove(sessionCharacter);
         }
      }
      else
      {
         var sessionCharacter = new SessionCharacter
         {
            SessionId = session.Id,
            Session = session,
            CharacterId = character.Id,
            Character = character,
            AppliedXP = 0
         };
         session.Characters.Add(sessionCharacter);
         dbContext.SessionCharacters.Add(sessionCharacter);
      }
   }
}
