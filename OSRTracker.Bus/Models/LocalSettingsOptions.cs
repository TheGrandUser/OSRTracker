using System;
using System.Collections.Generic;
using System.Text;

namespace OSRTracker.Models;

public class LocalSettingsOptions
{
    public string? ApplicationDataFolder
    {
        get; set;
    }

    public string? LocalSettingsFile
    {
        get; set;
    }
}
