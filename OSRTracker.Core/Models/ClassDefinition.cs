using System;
using System.Collections.Generic;
using System.Text;

namespace OSRTracker.Models;

public class ClassDefinition
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<LevelXPRequirement> LevelXP { get; set; } = [];
    public List<AttributeDefinition> KeyAttributes { get; set; } = [];
}


public record LevelXPRequirement(int XP);