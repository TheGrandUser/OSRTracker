using System;
using System.Collections.Generic;
using System.Text;
using OSRTracker.Core.Models;
using OSRTracker.Models;

namespace OSRTracker.Contracts.Services;

public interface IAppStateService
{
   string CampaignDbPath { get; }

   CampaignSettings? Campaign { get; }

   Task CreateCampaignAsync(string path, string campaignName, SystemDto? rpgSystem);
   Task OpenCampaignAsync(string path);
}
