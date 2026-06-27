using System;
using System.Collections.Generic;
using System.Text;
using OSRTracker.Helpers;

namespace OSRTracker.Models;

public readonly record struct GeneralXPAwardId(int Id) : IEntityId<GeneralXPAwardId>
{
   public static GeneralXPAwardId Empty { get; } = new(0);

   public static GeneralXPAwardId Create(int id) => new(id);
   public override string ToString() => $"General XP Award {Id}";
}

public class GeneralXPAward
{
    public GeneralXPAwardId Id { get; set; }
    public SessionDelveId SessionDelveId { get; set; }

    public required SessionDelve SessionDelve { get; set; }

    public int Amount { get; set; }
    public List<Character> Characters { get; set; } = [];

    public string Description { get; set; } = string.Empty;
}
