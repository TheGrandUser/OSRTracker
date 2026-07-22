using System.Diagnostics.CodeAnalysis;
using OSRTracker.Models;

namespace OSRTracker.ViewModels.Pages.GamePlay.Data;

public class TreasureEntryDto
{
   public TreasureEntryId Id { get; set; }

   public DelveId? DelveId { get; set; }

   public string Description { get; set; } = string.Empty;
   public int Quantity { get; set; }

   public decimal ApparentValue { get; set; }

   public LocationType LocationType { get; set; }
   public CharacterId? LocCharacterId { get; set; }
   public string LocStore { get; set; } = string.Empty;

   [MemberNotNullWhen(true, nameof(MagicItemIdentificationStatus))]
   [MemberNotNullWhen(true, nameof(MagicItemTrueValue))]
   public bool IsMagicItem => MagicItemIdentificationStatus.HasValue;

   public IdentificationStatus? MagicItemIdentificationStatus { get; set; }
   public int? MagicItemTrueValue { get; set; }


   public string Notes { get; set; } = string.Empty;
   public decimal Weight { get; set; }
}