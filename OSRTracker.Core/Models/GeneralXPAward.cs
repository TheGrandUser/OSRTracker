using System;
using System.Collections.Generic;
using System.Text;

namespace OSRTracker.Models;

public class GeneralXPAward
{
    public int Id { get; set; }
    public int SessionDelveId { get; set; }

    public required SessionDelve SessionDelve { get; set; }

    public int Amount { get; set; }
    public List<Character> Characters { get; set; } = [];

    public string Description { get; set; } = string.Empty;
}
