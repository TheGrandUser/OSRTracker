using System;
using System.Collections.Generic;
using System.Text;
using OSRTracker.Contracts.Services;

namespace OSRTracker.Services;

internal class AppInfoService : IAppInfoService
{
   public string GetAppVersion() => App.GetVersion();
}
