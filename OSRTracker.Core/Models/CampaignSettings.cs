using System;
using System.Collections.Generic;
using System.Text;
using OSRTracker.Contracts.Services;
using OSRTracker.Helpers;

namespace OSRTracker.Models;

public readonly record struct CampaignId(int Value) : IEntityId<CampaignId>
{
   public static CampaignId Empty { get; } = new(0);
   public static CampaignId Create(int id) => new(id);
   public override string ToString() => $"Campaign {Value}";
}

public class CampaignSettings
{
   public CampaignId Id { get; set; }

   public string Name { get; set; } = string.Empty;
   public string SystemName { get; set; } = string.Empty;
   public int XPForFirstLevel { get; set; } = 1000;
   public DelveCalculationMethod DelveCalcMethod { get; set; }


}
