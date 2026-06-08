using System;
using System.Collections.Generic;
using System.Text;

namespace OSRTracker.Core.Models;

public class Character
{
   public int Id { get; set; }
   public required string Name { get; set; }
   public string? PlayerName { get; set; }

   public CharacterType CharacterType { get; set; }
   public CharacterStatus Status { get; set; }
   public int? ClassId { get; set; }
   public ClassDefinition? Class { get; set; }
   public int Level { get; set; }
   public int CurrentXP { get; set; }

   public decimal ShareMultiplierXP { get; set; } = 1.0m;
   public decimal ShareMultiplierTreasure { get; set; } = 1.0m;

   public int Str { get; set; }
   public int Int { get; set; }
   public int Wis { get; set; }
   public int Dex { get; set; }
   public int Con { get; set; }
   public int Cha { get; set; }
}

public enum CharacterType { PC, Hireling, NPC }
public enum CharacterStatus { Active, Retired, Dead, PermanentlyDead }
