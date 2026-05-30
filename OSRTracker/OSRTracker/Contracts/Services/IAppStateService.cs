using System;
using System.Collections.Generic;
using System.Text;
using OSRTracker.Core.Models;

namespace OSRTracker.Contracts.Services;

public interface IAppStateService
{
   string CampaignDbPath { get; }

   CampaignSettings? Campaign { get; }

   Task CreateCampaignAsync(string path);
   Task OpenCampaignAsync(string path);
}
