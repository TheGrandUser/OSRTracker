using System;
using System.Collections.Generic;
using System.Text;
using OSRTracker.Helpers;

namespace OSRTracker.Models;

public readonly record struct TreasureEntryId(int Id) : IEntityId<TreasureEntryId>
{
   public static TreasureEntryId Empty { get; } = new(0);

   public static TreasureEntryId Create(int id) => new(id);
   public override string ToString() => $"Treasure Entry {Id}";
}

public class TreasureEntry
{
    public TreasureEntryId Id { get; set; }
    public SessionDelveId SessionDelveId { get; set; }
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
    public static LocationReference CarriedBy(int characterId) => new(LocationType.Character, characterId, null);
    public static LocationReference StoredIn(string storeDescription) => new(LocationType.Stored, null, storeDescription);
}
