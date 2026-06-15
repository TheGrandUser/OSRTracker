using System;
using System.Collections.Generic;
using System.Text;

namespace OSRTracker.Models;

public class TreasureEntry
{
    public int Id { get; set; }
    public int SessionDelveId { get; set; }
    public required SessionDelve SessionDelve { get; set; }

    public string Description { get; set; } = string.Empty;
    public int Quantiy { get; set; }

    public decimal ApparentValue { get; set; }
    public MagicItemDetails? MagicItemDetails { get; set; }

    public decimal Weight { get; set; }

    public LocationReference Location { get; set; } = null!;

    public string? Notes { get; set; }
}

public record MagicItemDetails(decimal? TrueValue, IdentificationStatus IdentificationStatus);

public enum IdentificationStatus
{
    Unidentified,
    PartiallyIdentified,
    FullyIdentified
}

public enum LocationType { Character, Stored }
public record LocationReference(LocationType Type, int? CharacterId, string? StoreDescription)
{
    public static LocationReference CarriedBy(int characterId) => new LocationReference(LocationType.Character, characterId, null);
    public static LocationReference StoredIn(string storeDescription) => new LocationReference(LocationType.Stored, null, storeDescription);
}
