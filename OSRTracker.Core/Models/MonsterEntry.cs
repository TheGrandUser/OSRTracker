using System;
using System.Collections.Generic;
using System.Text;
using OSRTracker.Helpers;

namespace OSRTracker.Models;

public readonly record struct MonsterEntryId(int Value) : IEntityId<MonsterEntryId>
{
   public static MonsterEntryId Empty { get; } = new(0);

   public static MonsterEntryId Create(int id) => new(id);
   public override string ToString() => $"Monster Entry {Value}";
}

public class MonsterEntry
{
   public MonsterEntryId Id { get; set; }
   public SessionId SessionId { get; set; }
   public required Session Session { get; set; }

   public DelveId? DelveId { get; set; }
   public Delve? Delve { get; set; }

   public required string Name { get; set; }
   public int Quantity { get; set; }

   public int XPValue { get; set; }
   public string? Notes { get; set; }
   public bool HasBeenApplied { get; set; }
}
