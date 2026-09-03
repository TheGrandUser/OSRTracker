using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using OSRTracker.Contracts.Messages;
using OSRTracker.Contracts.Services;
using OSRTracker.Contracts.ViewModels;
using OSRTracker.Data.Contracts.Services;
using OSRTracker.Models;
using OSRTracker.ViewModels.Pages.GamePlay.Data;
using Windows.Web.Http;

namespace OSRTracker.ViewModels.Pages.GamePlay;

public partial class GamePlayPageViewModel(
   IAppDbContextFactory dbContextFactory,
   IAppStateService appStateService,
   IGamePlayDataRepo gamePlayDataRepo,
   IXPCalculationService xpCalculationService,
   IDialogService dialogService,
   ITimeSource timeSource)
   : ObservableRecipient, INavigationAware
{
   SessionTrackItem? currentTrackItem;

   public SessionTrackItem? CurrentTrackItem {
      get => currentTrackItem;
      set {
         if (SetProperty(ref currentTrackItem, value))
         {
            if (currentTrackItem is not null)
            {
               SwitchToSession(currentTrackItem.Id);
            }
         }
      }
   }

   [ObservableProperty]
   public partial SessionTrackViewModel? CurrentTrack { get; private set; }

   [ObservableProperty]
   public partial SessionViewModel? CurrentSession { get; private set; }

   [ObservableProperty]
   public partial DelveViewModel? CurrentDelve { get; private set; }

   [ObservableProperty]
   public partial SessionDelveViewModel? CurrentSessionDelve { get; private set; }

   private readonly ObservableCollection<SessionTrackItem> sessionTracks = [];

   private readonly ObservableCollection<CharacterViewModel> characters = [];
   private readonly ObservableCollection<MonsterViewModel> monsters = [];
   private readonly ObservableCollection<GeneralXPViewModel> generalXP = [];
   private readonly ObservableCollection<TreasureViewModel> treasure = [];

   public IEnumerable<SessionTrackItem> SessionTracks => sessionTracks;

   public IEnumerable<CharacterViewModel> Characters => characters;
   public IEnumerable<MonsterViewModel> Monsters => monsters;
   public IEnumerable<GeneralXPViewModel> GeneralXP => generalXP;
   public IEnumerable<TreasureViewModel> Treasure => treasure;


   public void OnNavigatedFrom()
   {

   }

   public async void OnNavigatedTo(object parameter)
   {
      var tracks = await gamePlayDataRepo.GetSessionTracks();

      foreach (var t in tracks)
      {
         sessionTracks.Add(t);
      }

      SessionTrackId? activeSessionTrackId;

      if (parameter is SessionTrackId stId)
      {
         activeSessionTrackId = stId;
      }
      else
      {
         activeSessionTrackId = appStateService.ActiveSessionTrackId;

         if (!activeSessionTrackId.HasValue)
         {
            var defaultId = await gamePlayDataRepo.GetDefaultSessionTrackId();

            if (defaultId.Value != 0)
            {
               activeSessionTrackId = defaultId;
            }
         }
      }

      if (activeSessionTrackId.HasValue)
      {
         currentTrackItem = sessionTracks.First(sti => sti.Id == activeSessionTrackId.Value);
         OnPropertyChanged(nameof(CurrentTrackItem));
         await LoadSession(activeSessionTrackId.Value);
      }

   }


   private async void SwitchToSession(SessionTrackId id)
   {
      await LoadSession(id);
   }

   private async Task LoadSession(SessionTrackId sessionTrackId)
   {
      var gamePlayData = await gamePlayDataRepo.GetGamePlayDataAsync(sessionTrackId);
      if (gamePlayData is null)
      {
         monsters.Clear();
         treasure.Clear();
         generalXP.Clear();
         characters.Clear();

         CurrentSessionDelve = null;
         CurrentDelve = null;
         CurrentSession = null;
         CurrentTrack = null;

         return;
      }

      var sessionTrackData = gamePlayData.SessionTrack;

      CurrentTrack = new SessionTrackViewModel(sessionTrackData, dbContextFactory);

      var classes = gamePlayData.ClassDefinitions;

      characters.Clear();

      foreach (var characterData in gamePlayData.Characters)
      {
         var classDef = classes.FirstOrDefault(cd => cd.Id == characterData.ClassId);

         var vm = new CharacterViewModel(characterData, classDef, this, dbContextFactory);

         characters.Add(vm);
      }

      monsters.Clear();
      generalXP.Clear();
      treasure.Clear();

      foreach (var me in gamePlayData.MonsterEntries)
      {
         monsters.Add(new MonsterViewModel(me, dbContextFactory));
      }

      foreach (var ga in gamePlayData.GeneralXPAwards)
      {
         generalXP.Add(new GeneralXPViewModel(ga, dbContextFactory));
      }

      foreach (var te in gamePlayData.TreasureEntries)
      {
         treasure.Add(new TreasureViewModel(te, dbContextFactory, characters));
      }

      if (gamePlayData.Delve is not null)
      {
         CurrentDelve = new DelveViewModel(gamePlayData.Delve, dbContextFactory);
      }
      else
      {
         CurrentDelve = null;
      }

      if (gamePlayData.Session is not null)
      {
         CurrentSession = new SessionViewModel(gamePlayData.Session, dbContextFactory);
      }
      else
      {
         CurrentSession = null;
      }

      if (gamePlayData.SessionDelve is not null)
      {
         CurrentSessionDelve = new SessionDelveViewModel(gamePlayData.SessionDelve, dbContextFactory);
      }
      else
      {
         CurrentSessionDelve = null;
      }

   }

   protected override void OnPropertyChanged(PropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);


      if (e.PropertyName == nameof(CurrentTrack) || e.PropertyName == nameof(CurrentSession))
      {
         startNewSessionCommand?.NotifyCanExecuteChanged();
         concludeSessionCommand?.NotifyCanExecuteChanged();
         startNewDelveCommand?.NotifyCanExecuteChanged();
         concludeDelveCommand?.NotifyCanExecuteChanged();

      }
      else if (e.PropertyName == nameof(CurrentDelve))
      {
         startNewDelveCommand?.NotifyCanExecuteChanged();
         concludeDelveCommand?.NotifyCanExecuteChanged();
      }
   }

   private bool CanStartSession() => CurrentTrack is not null && CurrentSession is null;
   [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanStartSession))]
   private async Task StartNewSession()
   {
      using var dbContext = dbContextFactory.CreateDbContext();

      var trackId = CurrentTrack!.SessionTrackId;

      var sessionCount = await dbContext.Sessions.Where(s => s.SessionTrackId == trackId).CountAsync();

      var sessionNumber = "Session " + (sessionCount + 1);

      var result = await dialogService.GetInputAsync("Session Title", $"Title for session {sessionCount + 1}", "New Session");

      if (string.IsNullOrEmpty(result))
      {
         return;
      }

      var sessionTitle = result;

      var session = new Session()
      {
         SessionTrackId = trackId,
         Date = timeSource.GetUtcNow(),
         SessionNumber = sessionNumber,
         Title = sessionTitle,
         Status = SessionStatus.Active,
         SessionNotes = "",
      };

      dbContext.Sessions.Add(session);
      await dbContext.SaveChangesAsync();

      if (CurrentDelve is not null)
      {
         session.Delves.Add(new SessionDelve() { DelveId = CurrentDelve.DelveId, SessionId = session.Id });
         await dbContext.SaveChangesAsync();
      }

      var data = new SessionData() { SessionId = session.Id, SessionTitle = sessionTitle, SessionNumber = sessionNumber, SessionNotes = "" };

      CurrentSession = new SessionViewModel(data, dbContextFactory);
   }

   private bool CanConcludeSession() => CurrentTrack is not null && CurrentSession is not null;
   [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanConcludeSession))]
   private async Task ConcludeSession()
   {
      using var dbContext = await dbContextFactory.CreateDbContextAsync();

      var id = CurrentSession!.SessionId;

      var session = await ApplySessionXPDialogViewModel.GetData(dbContext, id);

      var campaignSettings = await dbContext.CampaignSettings.FirstAsync();

      using var dialogVM = new ApplySessionXPDialogViewModel(xpCalculationService, session, campaignSettings);

      var result = await dialogService.ShowDialogAsync(dialogVM);

      if (result != Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary)
      {
         return;
      }

      var xpApplications = dialogVM.Characters;

      foreach (var sc in session.Characters)
      {
         var xpApp = xpApplications.First(x => x.Character == sc.Character);

         sc.AppliedXP = xpApp.XP + xpApp.BonusXP;
      }

      session.Status = SessionStatus.Finished;

      await dbContext.SaveChangesAsync();

      CurrentSession.Dispose();
      CurrentSession = null;
   }

   private bool CanStartDelve() => CurrentTrack is not null && CurrentSession is not null && CurrentDelve is null;
   [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanStartDelve))]
   private async Task StartNewDelve()
   {
      var delveLocation = await dialogService.GetInputAsync("Delve Location", "Location Name for the Delve", "New Delve");

      if (delveLocation is null)
      {
         return;
      }

      using var dbContext = await dbContextFactory.CreateDbContextAsync();

      var trackId = CurrentTrack!.SessionTrackId;

      var delveCount = await dbContext.Delves.Where(d => d.SessionTrackId == trackId).CountAsync();

      var delve = new Delve()
      {
         SessionTrackId = trackId,
         LocationName = delveLocation,
         Status = DelveStatus.Active,
      };

      dbContext.Delves.Add(delve);
      await dbContext.SaveChangesAsync();

      if (CurrentSession is not null)
      {
         delve.Sessions.Add(new SessionDelve() { DelveId = delve.Id, SessionId = CurrentSession.SessionId });
         await dbContext.SaveChangesAsync();
      }

      var data = new DelveData() { DelveId = delve.Id, LocationName = delveLocation, LocationDescription = "" };

      CurrentDelve = new DelveViewModel(data, dbContextFactory);
   }

   private bool CanConcludeDelve() => CurrentTrack is not null && CurrentSession is not null && CurrentDelve is not null;
   [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanConcludeDelve))]
   private async Task ConcludeDelve()
   {
      using var dbContext = await dbContextFactory.CreateDbContextAsync();

      var id = CurrentDelve!.DelveId;

      var delve = await ApplyDelveXPDialogViewModel.GetDataAsync(dbContext, id);

      var campaignSettings = await dbContext.CampaignSettings.FirstAsync();

      using var dialogVM = new ApplyDelveXPDialogViewModel(xpCalculationService, delve, campaignSettings);

      var result = await dialogService.ShowDialogAsync(dialogVM);

      if (result != Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary)
      {
         return;
      }

      var xpApplications = dialogVM.Characters;

      foreach(var treasure in delve.Treasures)
      {
         if (treasure.MagicItemDetails is { }  magicDetails)
         {
            treasure.ApplicationStatus =
               (treasure.SaleStatus, magicDetails.IdentificationStatus) switch
               {
                  (TreasureSale.SoldWithoutUse, IdentificationStatus.FullyIdentified) => TreasureEntryApplicationStatus.Applied,
                  (TreasureSale.SoldWithoutUse, IdentificationStatus.PartiallyIdentified) => TreasureEntryApplicationStatus.Applied,

                  _ => TreasureEntryApplicationStatus.ApparentValueApplied,
               };
         }
         else
         {
            treasure.ApplicationStatus = TreasureEntryApplicationStatus.Applied;
         }
      }

      foreach (var xpApp in xpApplications)
      {
         var delveCharacter = delve.Characters.FirstOrDefault(dc => dc.CharacterId == xpApp.Character.Id);

         if(delveCharacter is null)
         {
            delveCharacter = new DelveCharacter() { CharacterId = xpApp.Character.Id, DelveId = delve.Id, AppliedXP = xpApp.XP + xpApp.BonusXP };
            delve.Characters.Add(delveCharacter);
         }
         else
         {
            delveCharacter.AppliedXP = xpApp.XP + xpApp.BonusXP;
         }
      }

      await dbContext.SaveChangesAsync();
   }

}
