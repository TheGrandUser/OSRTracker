using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Dapper;
using Microsoft.EntityFrameworkCore;
using OSRTracker.Contracts.Messages;
using OSRTracker.Contracts.Services;
using OSRTracker.Contracts.ViewModels;
using OSRTracker.Data;
using OSRTracker.Data.Contracts.Services;
using OSRTracker.Models;
using OSRTracker.ViewModels.Pages.GamePlay.Data;
using Windows.Web.Http;

namespace OSRTracker.ViewModels.Pages.GamePlay;

public partial class GamePlayViewModel(IAppDbContextFactory dbContextFactory, IAppStateService appStateService, IGamePlayDataRepo gamePlayDataRepo)
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
            using var dbContext = dbContextFactory.CreateDbContext();
            await dbContext.Database.OpenConnectionAsync();

            var connection = dbContext.Database.GetDbConnection();

            var id = await connection.QueryFirstOrDefaultAsync<int>("""SELECT st.Id FROM SessionTracks st ORDER BY st.Id LIMIT 1""");

            if (id != 0)
            {
               activeSessionTrackId = new SessionTrackId(id);
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

      var request = new InputTextRequest("Session Title", $"Title for session {sessionCount + 1}", "New Session");

      var result = await WeakReferenceMessenger.Default.Send(request);

      if (string.IsNullOrEmpty(result))
      {
         return;
      }

      var sessionTitle = result;

      var session = new Session()
      {
         SessionTrackId = trackId,
         Date = DateTime.UtcNow,
         SessionNumber = sessionNumber,
         Title = sessionTitle,
         Status = SessionStatus.Active,
         SessionNotes = "",
      };

      dbContext.Sessions.Add(session);
      await dbContext.SaveChangesAsync();

      var data = new SessionData() { SessionId = session.Id, SessionTitle = sessionTitle, SessionNumber = sessionNumber, SessionNotes = "" };

      CurrentSession = new SessionViewModel(data, dbContextFactory);
   }

   private bool CanConcludeSession() => CurrentTrack is not null && CurrentSession is not null;
   [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanConcludeSession))]
   private async Task ConcludeSession()
   {
      using var dbContext = dbContextFactory.CreateDbContext();

      var id = CurrentSession!.SessionId;

      var session = await dbContext.Sessions.Include(x => x.Characters).FirstAsync(x => x.Id == id);



      //characters
   }

   private bool CanStartDelve() => CurrentTrack is not null && CurrentSession is not null && CurrentDelve is null;
   [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanStartDelve))]
   private async Task StartNewDelve()
   {

   }

   private bool CanConcludeDelve() => CurrentTrack is not null && CurrentSession is not null && CurrentDelve is not null;
   [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanConcludeDelve))]
   private async Task ConcludeDelve()
   {

   }

}


public partial class SessionTrackViewModel(SessionTrackData data, IAppDbContextFactory dbContextFactory) : UpdateableElementViewModel(dbContextFactory)
{
   private string trackName = data.Name;
   public string TrackName { get => trackName; set => SetUpdatableProperty(ref trackName, value); }

   [ObservableProperty]
   public partial SessionTrackId SessionTrackId { get; set; } = data.Id;

   private string groupDescription = data.GroupDescription ?? string.Empty;
   public string GroupDescription { get => groupDescription; set => SetUpdatableProperty(ref groupDescription, value); }

   protected override void UpdateImpl(AppDbContext dbContext)
   {
      var sessionTrack = dbContext.SessionTracks.Find(SessionTrackId);

      if (sessionTrack is null)
      {
         return;
      }

      sessionTrack.Name = TrackName;
      sessionTrack.GroupDescription = GroupDescription;
   }
}

public partial class SessionViewModel(SessionData data, IAppDbContextFactory dbContextFactory) : UpdateableElementViewModel(dbContextFactory)
{
   private string sessionNumber = data.SessionNumber;
   private string sessionTitle = data.SessionTitle;
   private string sessionNotes = data.SessionNotes;

   public SessionId SessionId { get; } = data.SessionId;

   public string SessionNumber { get => sessionNumber; set => SetUpdatableProperty(ref sessionNumber, value); }

   public string SessionTitle { get => sessionTitle; set => SetUpdatableProperty(ref sessionTitle, value); }

   public string SessionNotes { get => sessionNotes; set => SetUpdatableProperty(ref sessionNotes, value); }


   protected override void UpdateImpl(AppDbContext dbContext)
   {
      var session = dbContext.Sessions.Find(SessionId);
      if (session is null)
      {
         return;
      }
      session.SessionNumber = SessionNumber;
      session.Title = SessionTitle;
      session.SessionNotes = SessionNotes;
   }
}

public partial class DelveViewModel(DelveData data, IAppDbContextFactory dbContextFactory) : UpdateableElementViewModel(dbContextFactory)
{
   private string locationDescription = data.LocationDescription;
   private string locationName = data.LocationName;
   public DelveId DelveId { get; } = data.DelveId;
   public string LocationDescription { get => locationDescription; set => SetUpdatableProperty(ref locationDescription, value); }
   public string LocationName { get => locationName; set => SetUpdatableProperty(ref locationName, value); }
   protected override void UpdateImpl(AppDbContext dbContext)
   {
      var delve = dbContext.Delves.Find(DelveId);
      if (delve is null)
      {
         return;
      }
      delve.LocationDescription = LocationDescription;
      delve.LocationName = LocationName;
   }
}

public partial class SessionDelveViewModel(SessionDelveData data, IAppDbContextFactory dbContextFactory) : UpdateableElementViewModel(dbContextFactory)
{
   private string notes = data.Notes;
   public SessionDelveId SessionDelveId { get; } = data.SessionDelveId;
   public string Notes { get => notes; set => SetUpdatableProperty(ref notes, value); }
   protected override void UpdateImpl(AppDbContext dbContext)
   {
      var sessionDelve = dbContext.SessionDelves.Find(SessionDelveId);
      if (sessionDelve is null)
      {
         return;
      }
      sessionDelve.Notes = Notes;
   }
}