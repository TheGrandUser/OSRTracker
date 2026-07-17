using System;
using System.Collections.Generic;
using System.Text;
using OSRTracker.Helpers;

namespace OSRTracker.Models;

public readonly record struct ClassDefinitionId(int Value) : IEntityId<ClassDefinitionId>
{
   public static ClassDefinitionId Empty { get; } = new(0);
   public static ClassDefinitionId Create(int id) => new(id);
   public override string ToString() => $"Class Definition {Value}";
}

public class ClassDefinition
{
   public ClassDefinitionId Id { get; set; }
   public string Name { get; set; } = string.Empty;
   public List<LevelXPRequirement> LevelXP { get; set; } = [];
   public List<AttributeDefinition> KeyAttributes { get; set; } = [];

   public (int floorXP, int nextXP) GetFloorAndNext(int level)
   {
      if (level == 0)
      {
         return (0, 500);
      }

      if (level >= LevelXP.Count)
      {
         return (LevelXP[^1].XP, int.MaxValue);
      }

      var levelIndex = level - 1;

      var floor = LevelXP[levelIndex].XP;
      var next = LevelXP[level].XP;

      return (floor, next);
   }
}


public record LevelXPRequirement(int XP);