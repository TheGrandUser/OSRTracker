using System;
using System.Collections.Generic;
using System.Text;

namespace OSRTracker.Models;

public class CurrencyDefinition
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal UnitValue { get; set; }
    public decimal CountPerUnitWeight { get; set; }

    public int Ordinal { get; set; }
}
