using OSRTracker.Models;

namespace OSRTracker.ViewModels.Pages.GamePlay.Data;

public class CharacterDto
{
   public CharacterId Id { get; set; }
   public string Name { get; set; } = string.Empty;
   public int XP { get; set; }

   public int Level { get; set; }
   public ClassDefinitionId? ClassId { get; set; }

   public bool InSession { get; set; }
   public decimal XPBonus { get; set; }
}
