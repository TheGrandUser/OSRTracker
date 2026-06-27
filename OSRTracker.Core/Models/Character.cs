using System;
using System.Collections.Generic;
using System.Text;
using OSRTracker.Helpers;

namespace OSRTracker.Models;

public readonly record struct CharacterId(int Id) : IEntityId<CharacterId>
{
   public static CharacterId Empty { get; } = new(0);

   public static CharacterId Create(int id) => new(id);
   public override string ToString() => $"Character {Id}";
}

public class Character
{
   public CharacterId Id { get; set; }
   public required string Name { get; set; }
   public string? PlayerName { get; set; }

   public CharacterType CharacterType { get; set; }
   public CharacterStatus Status { get; set; }
   public ClassDefinitionId? ClassId { get; set; }
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
