using System;
using System.Collections.Generic;
using System.Text;

namespace OSRTracker.Core.Models;

public class CampaignSettings
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string SystemName { get; set; } = string.Empty;
}
