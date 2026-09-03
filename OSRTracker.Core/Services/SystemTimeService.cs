using System;
using System.Collections.Generic;
using System.Text;
using OSRTracker.Contracts.Services;

namespace OSRTracker.Services;

public class SystemTimeService : ITimeSource
{
    public DateTime GetUtcNow()
    {
        return DateTime.UtcNow;
    }
}
