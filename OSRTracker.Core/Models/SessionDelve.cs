using System;
using System.Collections.Generic;
using System.Text;

namespace OSRTracker.Core.Models;

public class SessionDelve
{
    public int Id { get; set; }
    public required int SessionId { get; set; }
    public required int DelveId { get; set; }

    public required Session Session { get; set; }
    public required Delve Delve { get; set; }

    public string Notes { get; set; } = string.Empty;


    public List<TreasureEntry> Treasures { get; set; } = [];
    public List<MonsterEntry> Monsters { get; set; } = [];
    public List<GeneralXPAward> GeneralXPAwards { get; set; } = [];
}
