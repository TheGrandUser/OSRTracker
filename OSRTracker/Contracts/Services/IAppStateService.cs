using System;
using System.Collections.Generic;
using System.Text;
using OSRTracker.Models;

namespace OSRTracker.Contracts.Services;

public interface IAppStateService
{
   string CampaignDbPath { get; }

   CampaignSettings? Campaign { get; }
   SessionTrackId? ActiveSessionTrackId { get; }

   void SetActiveSessionTrackId(SessionTrackId? activeSessionTrackId);

   Task CreateCampaignAsync(string path, string campaignName, SystemDto? rpgSystem);
   Task<CampaignSettings> OpenCampaignAsync(string path);

   Task RollbackCampaignAsync(string path);
}
