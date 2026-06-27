using System;
using System.Collections.Generic;
using System.Text;
using OSRTracker.Helpers;

namespace OSRTracker.Models;

public readonly record struct MonsterEntryId(int Id) : IEntityId<MonsterEntryId>
{
   public static MonsterEntryId Empty { get; } = new(0);

   public static MonsterEntryId Create(int id) => new(id);
   public override string ToString() => $"Monster Entry {Id}";
}

public class MonsterEntry
{
    public MonsterEntryId Id { get; set; }
    public SessionDelveId SessionDelveId { get; set; }
    public required SessionDelve SessionDelve { get; set; }

    public required string Name { get; set; }
    public int Quantity { get; set; }

    public int XPValue { get; set; }
    public string? Notes { get; set; }
}
