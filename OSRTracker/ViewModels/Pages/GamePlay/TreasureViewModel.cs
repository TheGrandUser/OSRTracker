using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using OSRTracker.Data;
using OSRTracker.Data.Contracts.Services;
using OSRTracker.Models;
using OSRTracker.ViewModels.Pages.GamePlay.Data;

namespace OSRTracker.ViewModels.Pages.GamePlay;

public class TreasureLocationTypeViewModel(LocationType locationType, string label)
{
   public LocationType Value { get; } = locationType;
   public string Label { get; } = label;

   public static readonly TreasureLocationTypeViewModel[] Locations = [
      new TreasureLocationTypeViewModel(LocationType.Character, "Character"),
      new TreasureLocationTypeViewModel(LocationType.Stored, "Stored")
      ];
}

public class IdentificationVM(IdentificationStatus identificationStatus, string label)
{
   public IdentificationStatus Value { get; } = identificationStatus;
   public string Label { get; } = label;

   public static readonly IdentificationVM Unidentified = new(IdentificationStatus.Unidentified, "Unidentified");
   public static readonly IdentificationVM PartiallyIdentified = new(IdentificationStatus.PartiallyIdentified, "Partially Identified");
   public static readonly IdentificationVM FullyIdentified = new(IdentificationStatus.FullyIdentified, "Fully Identified");

   public static readonly IdentificationVM[] Identifications = [
      Unidentified, PartiallyIdentified, FullyIdentified,
      ];
}

public partial class TreasureViewModel : UpdateableElementViewModel
{
   private string description;
   private int quantity;
   private decimal apparentValue;

   private decimal trueValue;
   private IdentificationVM identificationStatus;

   private decimal weight;
   private TreasureLocationTypeViewModel locationType;
   private CharacterViewModel? locCharacter;
   private string storeLocation = string.Empty;

   private string notes;

   public TreasureViewModel(IAppDbContextFactory dbContextFactory)
      : base(dbContextFactory)
   {
      description = "";
      notes = "";
      locationType = TreasureLocationTypeViewModel.Locations[0];
      identificationStatus = IdentificationVM.FullyIdentified;
   }

   public TreasureViewModel(TreasureEntryDto data, IAppDbContextFactory dbContextFactory,
      ObservableCollection<CharacterViewModel> characters)
      : base(dbContextFactory)
   {
      Id = data.Id;
      DelveId = data.DelveId;

      description = data.Description;
      quantity = data.Quantity;

      apparentValue = data.ApparentValue;

      trueValue = data.MagicItemTrueValue ?? 0;
      identificationStatus = data.MagicItemIdentificationStatus.HasValue
         ? IdentificationVM.Identifications[(int)data.MagicItemIdentificationStatus.Value]
         : IdentificationVM.FullyIdentified;
      IsMagicItem = data.MagicItemIdentificationStatus.HasValue;

      weight = data.Weight;
      notes = data.Notes ?? string.Empty;

      locationType = TreasureLocationTypeViewModel.Locations[(int)data.LocationType];

      if (data.LocCharacterId is CharacterId holderId)
      {
         locCharacter = characters.FirstOrDefault(c => c.Id == holderId);
      }
      else
      {
         storeLocation = data.LocStore ?? string.Empty;
      }
   }

   public TreasureEntryId Id { get; }
   public DelveId? DelveId { get; }

   public string Description { get => description; set => SetUpdatableProperty(ref description, value); }
   public int Quantity { get => quantity; set => SetUpdatableProperty(ref quantity, value); }
   public double ApparentValue { get => (double)apparentValue; set => SetUpdatableProperty(ref apparentValue, (decimal)value); }
   public double Weight { get => (double)weight; set => SetUpdatableProperty(ref weight, (decimal)value); }

   public TreasureLocationTypeViewModel SelectedLocationType { get => locationType; set => SetUpdatableProperty(ref locationType, value); }
   public CharacterViewModel? Holder { get => locCharacter; set => SetUpdatableProperty(ref locCharacter, value); }
   public string StoreLocation { get => storeLocation; set => SetUpdatableProperty(ref storeLocation, value); }

   [ObservableProperty]
   [MemberNotNullWhen(true, nameof(TrueValue))]
   [MemberNotNullWhen(true, nameof(TrueValue))]
   public partial bool IsMagicItem { get; set; }

   public double TrueValue { get => IsMagicItem ? (double)trueValue : 0; set => SetUpdatableProperty(ref trueValue, (decimal)value); }
   public IdentificationVM Identification {
      get => IsMagicItem ? identificationStatus : IdentificationVM.FullyIdentified;
      set => SetUpdatableProperty(ref identificationStatus, value);
   }

   public string Notes { get => notes; set => SetUpdatableProperty(ref notes, value); }

   protected override void UpdateImpl(AppDbContext dbContext)
   {
      var entity = dbContext.TreasureEntries.Find(Id);

      if (entity is null)
      {
         return;
      }

      if (SelectedLocationType.Value == LocationType.Character && locCharacter is null)
      {
         return;
      }

      entity.Description = description;
      entity.Quantiy = quantity;
      entity.ApparentValue = apparentValue;

      if (IsMagicItem)
      {
         entity.MagicItemDetails = new MagicItemDetails(trueValue, identificationStatus.Value);
      }
      else
      {
         entity.MagicItemDetails = null;
      }

      entity.Weight = weight;

      entity.Location = SelectedLocationType.Value == LocationType.Character
         ? LocationReference.CarriedBy(locCharacter!.Id)
         : LocationReference.StoredIn(storeLocation);

      entity.Notes = notes;
   }
}
