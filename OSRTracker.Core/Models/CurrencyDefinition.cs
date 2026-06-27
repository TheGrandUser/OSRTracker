using System;
using System.Collections.Generic;
using System.Text;
using OSRTracker.Helpers;

namespace OSRTracker.Models;

public readonly record struct CurrencyDefinitionId(int Id) : IEntityId<CurrencyDefinitionId>
{
   public static CurrencyDefinitionId Empty { get; } = new(0);
   public static CurrencyDefinitionId Create(int id) => new(id);
   public override string ToString() => $"Currency Definition {Id}";
}

public class CurrencyDefinition
{
    public CurrencyDefinitionId Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal UnitValue { get; set; }
    public decimal CountPerUnitWeight { get; set; }

    public int Ordinal { get; set; }
}
