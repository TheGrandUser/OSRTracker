using System;
using System.Collections.Generic;
using System.Text;

namespace OSRTracker.Core.Models;

public class MonsterEntry
{
    public int Id { get; set; }
    public int SessionDelveId { get; set; }
    public required SessionDelve SessionDelve { get; set; }

    public required string Name { get; set; }
    public int Quantity { get; set; }

    public int XPValue { get; set; }
    public string? Notes { get; set; }
}
