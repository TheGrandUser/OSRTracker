using System;
using System.Collections.Generic;
using System.Text;
using OSRTracker.Helpers;

namespace OSRTracker.Models;

public readonly record struct DelveId(int Value) : IEntityId<DelveId>
{
   public static DelveId Empty { get; } = new(0);
   public static DelveId Create(int id) => new(id);
   public override string ToString() => $"Delve {Value}";
}

public class Delve
{
   public DelveId Id { get; set; }
   public required string LocationName { get; set; }

   public string LocationDescription { get; set; } = string.Empty;

   public DelveStatus Status { get; set; }

   public List<SessionDelve> Sessions { get; set; } = [];

   public SessionTrack? SessionTrack { get; set; }
   public required SessionTrackId SessionTrackId { get; set; }

   public List<TreasureEntry> Treasures { get; set; } = [];
   public List<MonsterEntry> Monsters { get; set; } = [];
   public List<GeneralXPAward> GeneralXPAwards { get; set; } = [];

   public List<DelveCharacter> Characters { get; set; } = [];
}

public enum DelveStatus
{
   Active,
   Completed,
}

public class DelveCharacter
{
   public required DelveId DelveId { get; set; }
   public Delve Delve { get; set; } = null!;
   public required CharacterId CharacterId { get; set; }
   public Character Character { get; set; } = null!;


   public int AppliedXP { get; set; }
}