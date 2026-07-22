using OSRTracker.Models;

namespace OSRTracker.ViewModels.Pages.GamePlay.Data;

public class ClassDefinitionDto
{
   public ClassDefinitionId Id { get; set; }
   public required string Name { get; set; }
   public required LevelXPRequirement[] LevelXP { get; set; }
}
