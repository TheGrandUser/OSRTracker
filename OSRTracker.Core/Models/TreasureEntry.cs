using System;
using System.Collections.Generic;
using System.Text;
using OSRTracker.Helpers;

namespace OSRTracker.Models;

public readonly record struct TreasureEntryId(int Value) : IEntityId<TreasureEntryId>
{
   public static TreasureEntryId Empty { get; } = new(0);

   public static TreasureEntryId Create(int id) => new(id);
   public override string ToString() => $"Treasure Entry {Value}";
}

public enum TreasureEntryApplicationStatus
{
   NotApplied,
   Applied,
   ApparentValueApplied
}

public enum TreasureSale
{
   NoSold,
   Sold,
   SoldWithoutUse
}

public class TreasureEntry
{
   public TreasureEntryId Id { get; set; }
   public SessionId SessionId { get; set; }
   public required Session Session { get; set; }

   public DelveId? DelveId { get; set; }
   public Delve? Delve { get; set; }

   public string Description { get; set; } = string.Empty;
   public int Quantiy { get; set; }

   public decimal Value { get; set; }
   public MagicItemDetails? MagicItemDetails { get; set; }

   public decimal Weight { get; set; }

   public LocationReference Location { get; set; } = null!;

   public string? Notes { get; set; }

   public TreasureEntryApplicationStatus ApplicationStatus { get; set; }
   public TreasureSale SaleStatus { get; set; }
}

public record MagicItemDetails(decimal ApparentValue, IdentificationStatus IdentificationStatus);

public enum IdentificationStatus
{
   Unidentified,
   PartiallyIdentified,
   FullyIdentified
}

public enum LocationType { Character, Stored }
public record LocationReference(LocationType Type, CharacterId? CharacterId, string? StoreDescription)
{
   public static LocationReference CarriedBy(CharacterId characterId) => new(LocationType.Character, characterId, null);
   public static LocationReference StoredIn(string storeDescription) => new(LocationType.Stored, null, storeDescription);
}
