using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI.Collections;
using Dapper;
using Microsoft.EntityFrameworkCore;
using OSRTracker.Contracts.ViewModels;
using OSRTracker.Core.Models;
using OSRTracker.ViewModels;
using OSRTracker.Data;
using OSRTracker.Data.Contracts.Services;
using OSRTracker;
using System.Diagnostics;

namespace OSRTracker.ViewModels.Pages;

public partial class CharacterRosterViewModel : ObservableRecipient, INavigationAware
{
   private AppDbContext? dbContext;

   private readonly List<AvailableClass> availableClasses = [];
   private readonly ObservableCollection<CharacterVMWrapper> characters = [];
   private readonly IAppDbContextFactory appDbContextFactory;
   private CancellationTokenSource? pageLifetimeCTS;
   private AdvancedCollectionView charactersView;

   public CharacterRosterViewModel(IAppDbContextFactory appDbContextFactory)
   {
      this.appDbContextFactory = appDbContextFactory;

      charactersView = new AdvancedCollectionView(characters);
   }

   public ObservableCollection<CharacterVMWrapper> Characters => characters;
   public AdvancedCollectionView CharactersView => charactersView;

   public List<AvailableClass> AvailableClasses => availableClasses;

   [ObservableProperty]
   public partial string StrLabel { get; set; } = "Str";
   [ObservableProperty]
   public partial string IntLabel { get; set; } = "Int";
   [ObservableProperty]
   public partial string WisLabel { get; set; } = "Wis";
   [ObservableProperty]
   public partial string DexLabel { get; set; } = "Dex";
   [ObservableProperty]
   public partial string ConLabel { get; set; } = "Con";
   [ObservableProperty]
   public partial string ChaLabel { get; set; } = "Cha";

   public void OnNavigatedFrom()
   {
      pageLifetimeCTS?.Cancel();
      pageLifetimeCTS?.Dispose();

      dbContext?.Dispose();
      dbContext = null;
   }
   public void OnNavigatedTo(object parameter)
   {
      pageLifetimeCTS = new CancellationTokenSource();

      PopulateCharacters();
   }

   private async void PopulateCharacters()
   {
      if (!appDbContextFactory.HasPath)
      {
         // Handle the case where the database path is not available

         return;
      }

      dbContext = appDbContextFactory.CreateDbContext();

      var connection = dbContext.Database.GetDbConnection();

      var classes = await connection.QueryAsync<AvailableClass>("""
         SELECT c.Id, c.Name
         FROM ClassDefinitions c
         ORDER BY c.Name
         """);

      availableClasses.AddRange(classes);

      var attributes = await connection.QueryAsync<(string Name, int Ordinal)>("""
         SELECT a.Name, a.Ordinal
         FROM AttributeDefinitions a
         ORDER BY a.Ordinal
         """);

      foreach (var (Name, Ordinal) in attributes)
      {
         switch (Ordinal)
         {
            case 1: StrLabel = Name; break;
            case 2: IntLabel = Name; break;
            case 3: WisLabel = Name; break;
            case 4: DexLabel = Name; break;
            case 5: ConLabel = Name; break;
            case 6: ChaLabel = Name; break;
            default: break;
         }
      }


      await foreach (var character in dbContext.Characters.AsAsyncEnumerable())
      {
         var vm = new CharacterVMWrapper(dbContext, availableClasses, new CharacterViewModel(character, dbContext, availableClasses));
         //var wrapper = new CharacterViewModelWrapper();

         characters.Add(vm);
      }

      var blank = new CharacterVMWrapper(dbContext, availableClasses);
      characters.Add(blank);

      charactersView = new AdvancedCollectionView(characters, true);

      blank.Realized += Blank_Realized;
   }

   private void Blank_Realized(object? sender, EventArgs e)
   {
      var prior = (CharacterVMWrapper)sender!;
      prior.Realized -= Blank_Realized;

      var blank = new CharacterVMWrapper(dbContext!, availableClasses);
      characters.Add(blank);
   }
}



public class AvailableClass
{
   public required int Id { get; set; }
   public required string Name { get; set; }
}


public partial class CharacterViewModel : UpdateableElementViewModel
{
   private readonly Character character;
   private string name;
   private string? playerName;
   private CharacterTypeViewModel? characterType;
   private CharacterStatusViewModel? characterStatus;
   private AvailableClass? @class;
   private int level;
   private int currentXP;
   private decimal shareMultiplierXP;
   private decimal shareMultiplierTreasure;
   private int str;
   private int @int;
   private int wis;
   private int dex;
   private int con;
   private int cha;

   public CharacterViewModel(Character character, AppDbContext appDbContext, List<AvailableClass> availableClasses)
      : base(appDbContext)
   {
      this.character = character;

      name = this.character.Name;
      playerName = this.character.PlayerName;

      characterType = CharacterTypeViewModel.CharacterTypes[(int)this.character.CharacterType];
      characterStatus = CharacterStatusViewModel.CharacterStatuses[(int)this.character.Status];

      @class =
         this.character.ClassId.HasValue
         ? availableClasses.FirstOrDefault(c => c.Id == this.character.ClassId)
         : null;

      level = this.character.Level;
      currentXP = this.character.CurrentXP;


      shareMultiplierXP = this.character.ShareMultiplierXP;
      shareMultiplierTreasure = this.character.ShareMultiplierTreasure;

      str = this.character.Str;
      @int = this.character.Int;
      wis = this.character.Wis;
      dex = this.character.Dex;
      con = this.character.Con;
      cha = this.character.Cha;

   }

   public string Name { get => name; set => SetUpdatableProperty(ref name, value); }
   public string? PlayerName { get => playerName; set => SetUpdatableProperty(ref playerName, value); }

   public CharacterTypeViewModel? CharacterType { get => characterType; set => SetUpdatableProperty(ref characterType, value); }
   public CharacterStatusViewModel? CharacterStatus { get => characterStatus; set => SetUpdatableProperty(ref characterStatus, value); }
   public AvailableClass? Class { get => @class; set => SetUpdatableProperty(ref @class, value); }
   public int Level { get => level; set => SetUpdatableProperty(ref level, value); }
   public int CurrentXP { get => currentXP; set => SetUpdatableProperty(ref currentXP, value); }
   public double ShareMultiplierXP { get => (double)shareMultiplierXP; set => SetUpdatableProperty(ref shareMultiplierXP, (decimal)value); }
   public double ShareMultiplierTreasure { get => (double)shareMultiplierTreasure; set => SetUpdatableProperty(ref shareMultiplierTreasure, (decimal)value); }

   public int Str { get => str; set => SetUpdatableProperty(ref str, value); }
   public int Int { get => @int; set => SetUpdatableProperty(ref @int, value); }
   public int Wis { get => wis; set => SetUpdatableProperty(ref wis, value); }
   public int Dex { get => dex; set => SetUpdatableProperty(ref dex, value); }
   public int Con { get => con; set => SetUpdatableProperty(ref con, value); }
   public int Cha { get => cha; set => SetUpdatableProperty(ref cha, value); }

   protected override void UpdateImpl(AppDbContext dbContext)
   {
      character.Name = Name;
      character.PlayerName = PlayerName;
      character.CharacterType = CharacterType?.CharacterType ?? Core.Models.CharacterType.PC;
      character.ClassId = Class?.Id;
      character.Level = Level;
      character.CurrentXP = CurrentXP;

      character.ShareMultiplierXP = shareMultiplierXP;
      character.ShareMultiplierTreasure = shareMultiplierTreasure;

      character.Str = Str;
      character.Int = Int;
      character.Wis = Wis;
      character.Dex = Dex;
      character.Con = Con;
      character.Cha = Cha;
   }
}

public class CharacterTypeViewModel(CharacterType characterType, string name)
{
   public string Name { get; } = name;
   public CharacterType CharacterType { get; } = characterType;

   public static ImmutableList<CharacterTypeViewModel> CharacterTypes { get; } = [
      new CharacterTypeViewModel(CharacterType.PC, "PC"),
      new CharacterTypeViewModel(CharacterType.Hireling, "Hireling"),
      new CharacterTypeViewModel(CharacterType.NPC, "NPC"),
      ];
}

public class CharacterStatusViewModel(CharacterStatus characterStatus, string name)
{
   public string Name { get; } = name;
   public CharacterStatus CharacterStatus { get; } = characterStatus;

   public static ImmutableList<CharacterStatusViewModel> CharacterStatuses { get; } = [
      new CharacterStatusViewModel(CharacterStatus.Active, "Active"),
      new CharacterStatusViewModel(CharacterStatus.Retired, "Retired"),
      new CharacterStatusViewModel(CharacterStatus.Dead, "Dead"),
      new CharacterStatusViewModel(CharacterStatus.PermanentlyDead, "Permanently Dead"),
      ];
}

[GenerateRowWrapper]
public partial class CharacterVMWrapper : RowWrapperBase<CharacterViewModel>
{
   private readonly AppDbContext dbContext;
   private readonly List<AvailableClass> availableClasses;

   public CharacterVMWrapper(AppDbContext dbContext, List<AvailableClass> availableClasses, CharacterViewModel? initial = null) : base(initial)
   {
      this.dbContext = dbContext;
      this.availableClasses = availableClasses;
   }

   protected override CharacterViewModel Create()
   {
      var character = new Character()
      {
         Name = "",
         Class = null,
         ClassId = null,
         CharacterType = Core.Models.CharacterType.PC,
         Status = Core.Models.CharacterStatus.Active,
      };

      var entry = dbContext.Characters.Add(character);

      if (entry is not null)
      {
         Debug.Assert(entry.State == EntityState.Added);
      }

      dbContext.SaveChanges();

      var vm = new CharacterViewModel(character, dbContext, availableClasses);

      return vm;
   }
}