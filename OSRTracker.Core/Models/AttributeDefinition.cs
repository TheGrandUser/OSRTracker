using System.Collections.Immutable;
using OSRTracker.Helpers;

namespace OSRTracker.Models;

public readonly record struct AttributeDefinitionId(int Id) : IEntityId<AttributeDefinitionId>
{
   public static AttributeDefinitionId Empty { get; } = new(0);
   public static AttributeDefinitionId Create(int id) => new(id);

   public override string ToString() => $"Attribute Definition {Id}";
}

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
   public AttributeDefinitionId Id { get; set; }
   public string Name { get; set; } = string.Empty;
   public int Ordinal { get; set; }
}

