using OSRTracker.Models;

namespace OSRTracker.ViewModels.Pages.Main;

public class CharacterSummaryVM
{
   public required CharacterId Id { get; set; }
   public required string Name { get; set; }
   public string? ClassName { get; set; }
   public required int Level { get; set; }
}

