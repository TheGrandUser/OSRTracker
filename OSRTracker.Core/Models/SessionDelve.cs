using System;
using System.Collections.Generic;
using System.Text;
using OSRTracker.Helpers;

namespace OSRTracker.Models;

public readonly record struct SessionDelveId(int Id) : IEntityId<SessionDelveId>
{
   public static SessionDelveId Empty { get; } = new(0);

   public static SessionDelveId Create(int id) => new(id);
   public override string ToString() => $"Session Delve {Id}";
}

public class SessionDelve
{
   public SessionDelveId Id { get; set; }
   public required SessionId SessionId { get; set; }
   public required DelveId DelveId { get; set; }

   public required Session Session { get; set; }
   public required Delve Delve { get; set; }

   public string Notes { get; set; } = string.Empty;


   public List<TreasureEntry> Treasures { get; set; } = [];
   public List<MonsterEntry> Monsters { get; set; } = [];
   public List<GeneralXPAward> GeneralXPAwards { get; set; } = [];
}
