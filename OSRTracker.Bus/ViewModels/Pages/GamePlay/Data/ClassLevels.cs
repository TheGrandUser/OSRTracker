using OSRTracker.Models;

namespace OSRTracker.ViewModels.Pages.GamePlay.Data;

public class ClassLevels
{
   public ClassDefinitionId Id { get; set; }
   public required string Name { get; set; }
   public required LevelXPRequirement[] LevelXP { get; set; }

   public (int floorXP, int nextXP) GetFloorAndNext(int level)
   {
      if (level == 0)
      {
         return (0, 500);
      }

      if (level >= LevelXP.Length)
      {
         return (LevelXP[^1].XP, int.MaxValue);
      }

      var levelIndex = level - 1;

      var floor = LevelXP[levelIndex].XP;
      var next = LevelXP[level].XP;

      return (floor, next);
   }
}
