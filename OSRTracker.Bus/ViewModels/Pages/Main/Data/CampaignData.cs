using System;
using System.Collections.Generic;
using System.Text;

namespace OSRTracker.ViewModels.Pages.Main.Data;

public class CampaignData
{
   public required CampaignInfo CampaignInfo { get; set; }
   public required List<CharacterSummary> Characters { get; set; }
   public required List<SessionTrackData> SessionTracks { get; set; }
}
