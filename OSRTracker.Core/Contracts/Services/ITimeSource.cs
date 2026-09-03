using System;
using System.Collections.Generic;
using System.Text;

namespace OSRTracker.Contracts.Services;

public interface ITimeSource
{
   DateTime GetUtcNow();
}
