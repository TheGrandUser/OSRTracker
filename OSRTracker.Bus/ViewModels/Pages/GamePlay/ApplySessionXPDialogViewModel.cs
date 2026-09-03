using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using OSRTracker.Contracts.Services;
using OSRTracker.Data;
using OSRTracker.Data.Contracts.Services;
using OSRTracker.Models;
using OSRTracker.ViewModels.Pages.Main;

namespace OSRTracker.ViewModels.Pages.GamePlay;

public partial class ApplySessionXPDialogViewModel : DialogViewModel
{
   private readonly IXPCalculationService xpCalculationService;
   private readonly Session session;
   private readonly List<Character> characters;

   private readonly List<CharacterXPApplicationVM> xpApplications = [];

   public string SessionTitle => session.Title ?? "";
   public string SessionNumber => session.SessionNumber;

   public IEnumerable<CharacterXPApplicationVM> Characters => xpApplications;

   public static async Task<Session> GetData(AppDbContext dbContext, SessionId sessionId)
   {
      var session = await dbContext.Sessions
         .AsSplitQuery()
         .Include(x => x.Characters)
         .ThenInclude(sc => sc.Character)
         .ThenInclude(c => c.Class)
         .Include(x => x.GeneralXPAwards)
         .Include(x => x.Monsters)
         .Include(x => x.Treasures.Where(t => t.DelveId == null))
         .FirstAsync(x => x.Id == sessionId);

      return session;
   }

   public ApplySessionXPDialogViewModel(IXPCalculationService xpCalculationService, Session session, CampaignSettings campaignSettings)
   {
      this.xpCalculationService = xpCalculationService;
      this.session = session;

      var delvelessTreasure = session.Treasures.Where(t => t.DelveId == null).ToList();

      characters = [.. session.Characters.Select(sc => sc.Character!)];

      var xpApplications = this.xpCalculationService.CalculateSessionChanges(characters, session.GeneralXPAwards, session.Monsters, delvelessTreasure);

      foreach (var xpApp in xpApplications)
      {
         this.xpApplications.Add(new CharacterXPApplicationVM(xpApp, campaignSettings));
      }
   }

   protected async override Task OnPrimaryExecuted()
   {
   }
}

public partial class CharacterXPApplicationVM(XPApplication xpApplication, CampaignSettings campaignSettings) : ObservableObject
{
   public Character Character => xpApplication.Character;

   private int xp = xpApplication.Change;

   public string Name => xpApplication.Character.Name;

   public int XP { get => xp; set => SetProperty(ref xp, value); }

   public int CurrentXP => xpApplication.Character.CurrentXP;

   public int NextXP => xpApplication.Character.Class?.GetFloorAndNext(xpApplication.Character.Level).nextXP ?? campaignSettings.XPForFirstLevel;

   [ObservableProperty]
   public partial int BonusXP { get; set; }
}
