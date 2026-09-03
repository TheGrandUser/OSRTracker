using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.EntityFrameworkCore;
using OSRTracker.Contracts.Services;
using OSRTracker.Data;
using OSRTracker.Models;
using static System.Collections.Specialized.BitVector32;

namespace OSRTracker.ViewModels.Pages.GamePlay;

public partial class ApplyDelveXPDialogViewModel : DialogViewModel
{
   private readonly IXPCalculationService xpCalculationService;
   private readonly Delve delve;
   private readonly CampaignSettings campaignSettings;
   private readonly List<Character> characters;
   private DelveCalculationMethodVM selectedMethod;

   private readonly ObservableCollection<CharacterXPApplicationVM> xpApplications = [];
   private readonly List<(SessionId, List<CharacterId>)> characterPresences;

   public string LocationName => delve.LocationName;

   public DelveCalculationMethodVM SelectedMethod {
      get => selectedMethod;
      set {
         if (SetProperty(ref selectedMethod, value))
         {
            SetXPCalcMethod(selectedMethod.Method);
         }
      }
   }

   public IEnumerable<CharacterXPApplicationVM> Characters => xpApplications;

   public static async Task<Delve> GetDataAsync(AppDbContext dbContext, DelveId delveId)
   {
      var delve = await dbContext.Delves
         .AsSplitQuery()
         .Include(d => d.Characters)
         .Include(d => d.Treasures)
         .Include(d => d.Sessions)
         .ThenInclude(sd => sd.Session)
         .ThenInclude(s => s.Characters)
         .ThenInclude(sc => sc.Character)
         .ThenInclude(c => c.Class)
         .FirstAsync(d => d.Id == delveId);

      return delve;
   }

   public ApplyDelveXPDialogViewModel(IXPCalculationService xpCalculationService, Delve delve, CampaignSettings campaignSettings)
   {
      this.xpCalculationService = xpCalculationService;
      this.delve = delve;
      this.campaignSettings = campaignSettings;

      selectedMethod = DelveCalculationMethodVM.AvailableMethods[(int)campaignSettings.DelveCalcMethod];

      characters = [.. delve.Sessions.SelectMany(sd => sd.Session.Characters).Select(sc => sc.Character)];
      characterPresences = delve.Sessions.Select(sd => (sd.SessionId, sd.Session.Characters.Select(sc => sc.CharacterId).ToList())).ToList();

      SetXPCalcMethod(campaignSettings.DelveCalcMethod);
   }

   private void SetXPCalcMethod(DelveCalculationMethod method)
   {
      var xpApplications =
         method switch
         {
            DelveCalculationMethod.AnyAfterAcquisition => xpCalculationService.CalculateDelveChanges_AnyAfterAcquisition(characters, characterPresences, delve.Treasures),
            DelveCalculationMethod.ProportionalAfterAcquisition => xpCalculationService.CalculateDelveChanges_ProportionalAfterAcquisition(characters, characterPresences, delve.Treasures),
            DelveCalculationMethod.ProportionalParticipation => xpCalculationService.CalculateDelveChanges_ProportionalParticipation(characters, characterPresences, delve.Treasures),
            _ => xpCalculationService.CalculateDelveChanges_AnyParticipation(characters, delve.Treasures)
         };


      this.xpApplications.Clear();
      foreach (var xpApp in xpApplications)
      {
         this.xpApplications.Add(new CharacterXPApplicationVM(xpApp, campaignSettings));
      }
   }

   protected override Task OnPrimaryExecuted() => Task.CompletedTask;
}

public class DelveCalculationMethodVM
{
   public DelveCalculationMethod Method { get; set; }
   public string Name { get; set; } = string.Empty;

   public static readonly DelveCalculationMethodVM[] AvailableMethods = [
      new DelveCalculationMethodVM() { Method = DelveCalculationMethod.AnyParticipation, Name = "Any Participation" },
      new DelveCalculationMethodVM() { Method = DelveCalculationMethod.AnyAfterAcquisition, Name = "Any After Acquisition" },
      new DelveCalculationMethodVM() { Method = DelveCalculationMethod.ProportionalParticipation, Name = "Proportional Participation" },
      new DelveCalculationMethodVM() { Method = DelveCalculationMethod.ProportionalAfterAcquisition, Name = "Proportional After Acquisition" },
      ];
}