using System;
using System.Collections.Generic;
using System.Text;
using OSRTracker.Helpers;

namespace OSRTracker.Models;

public readonly record struct ClassDefinitionId(int Id) : IEntityId<ClassDefinitionId>
{
   public static ClassDefinitionId Empty { get; } = new(0);
   public static ClassDefinitionId Create(int id) => new(id);
   public override string ToString() => $"Class Definition {Id}";
}

public class ClassDefinition
{
    public ClassDefinitionId Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<LevelXPRequirement> LevelXP { get; set; } = [];
    public List<AttributeDefinition> KeyAttributes { get; set; } = [];
}


public record LevelXPRequirement(int XP);