using System.Collections.Immutable;

namespace OSRTracker.Core.Models;

public class AttributeDefinition
{
   public static ImmutableList<AttributeDefinition> Defaults { get; } = [
      new AttributeDefinition() { Name = "Str", Ordinal = 1 },
      new AttributeDefinition() { Name = "Int", Ordinal = 2 },
      new AttributeDefinition() { Name = "Wis", Ordinal = 3 },
      new AttributeDefinition() { Name = "Dex", Ordinal = 4 },
      new AttributeDefinition() { Name = "Con", Ordinal = 5 },
      new AttributeDefinition() { Name = "Cha", Ordinal = 6 },
      ];
   public int Id {
      get; set;
   }
   public string Name { get; set; } = string.Empty;
   public int Ordinal { get; set; }
}
