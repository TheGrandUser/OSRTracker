using System;
using System.Collections.Generic;
using System.Text;

namespace OSRTracker.Models;

public class Delve
{
    public int Id { get; set; }
    public required string LocationName { get; set; }

    public DelveStatus Status { get; set; }

    public List<SessionDelve> Sessions { get; set; } = [];
}

public enum DelveStatus
{
    Active,
    Completed,
}
