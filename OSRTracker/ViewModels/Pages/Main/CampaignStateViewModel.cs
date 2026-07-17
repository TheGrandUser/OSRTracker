using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Controls;
using OSRTracker.Contracts.Messages;
using OSRTracker.Contracts.Services;
using OSRTracker.Data.Contracts.Services;
using OSRTracker.Models;
using Windows.UI.Popups;

namespace OSRTracker.ViewModels.Pages.Main;

public partial class CampaignStateViewModel : MainStateViewModel
{
   private readonly IAppDbContextFactory dbContextFactory;
   private readonly IAppStateService appStateService;

   private readonly ObservableCollection<SessionTrackVM> sessionTracks = [];
   private readonly ObservableCollection<CharacterSummaryVM> characterSummaries = [];

   private SessionTrackVM? activeTrack;

   public CampaignStateViewModel(
      IAppDbContextFactory dbContextFactory,
      IAppStateService appStateService,
      CampaignInfo campaignInfo,
      List<CharacterSummaryVM> characters,
      List<SessionTrackVM> sessionTracks)
   {
      this.dbContextFactory = dbContextFactory;
      this.appStateService = appStateService;

      //   Open Sessions (technically there could be multiple, say in a pbp game with multiple groups, but will usually just be one if the app is restarted in the middle of a session)
      //     Players/characters involved, current delve, basic description

      // ? Open Delves (simpler to have even in a game with one group if the players can have multiple parties)
      //     Players/characters involved

      //   Most recent session

      //   Start new session (start new delve would happen in the session screen)

      // Probaby should add a session track to hold sesssions and player groups

      Name = campaignInfo.Name;
      SystemName = campaignInfo.SystemName;

      foreach (var c in characters)
      {
         characterSummaries.Add(c);
      }

      foreach (var st in sessionTracks)
      {
         this.sessionTracks.Add(st);
      }

      activeTrack = this.sessionTracks.FirstOrDefault();

      WeakReferenceMessenger.Default.Register<ActiveSessionTrack>(this, OnActiveSessionChanged);
   }

   private void OnActiveSessionChanged(object recipient, ActiveSessionTrack message)
   {
      foreach (var vm in sessionTracks)
      {
         vm.OnActiveSessionChanged();
      }
   }

   public ObservableCollection<SessionTrackVM> SessionTracks => sessionTracks;

   [MaybeNull]
   public SessionTrackVM ActiveTrack {
      get => activeTrack;
      set {
         if (SetProperty(ref activeTrack, value))
         {
            appStateService.SetActiveSessionTrackId(activeTrack?.Id);
         }
      }
   }

   public ObservableCollection<CharacterSummaryVM> CharacterSummaries => characterSummaries;

   [ObservableProperty]
   public partial string Name { get; set; }
   [ObservableProperty]
   public partial string SystemName { get; set; }

   public static async Task<CampaignStateViewModel> CreateAsync(IAppDbContextFactory dbContextFactory, IAppStateService appStateService)
   {
      using var dbContext = await dbContextFactory.CreateDbContextAsync();

      await dbContext.Database.OpenConnectionAsync();

      var connection = dbContext.Database.GetDbConnection();

      var campaignInfo = await connection.QueryFirstAsync<CampaignInfo>("""
            SELECT c.Name, c.SystemName
            FROM CampaignSettings c
            WHERE c.Id = 1
            """);

      var characters = await connection.QueryAsync<CharacterSummaryVM>("""
            SELECT c.Id, c.Name, IfNull(cd.Name, "None") AS ClassName, c.Level
            FROM Characters as c
            LEFT JOIN ClassDefinitions AS cd
            ON cd.Id = c.ClassId
            WHERE c.Status = 0 AND c.CharacterType IN (0, 1)
            ORDER BY c.Name ASC
            """);

      List<CharacterSummaryVM> characterSummaries = [];

      foreach (var characterSummary in characters)
      {
         characterSummaries.Add(characterSummary);
      }


      var sessionTrackDatas = (await connection.QueryAsync<SessionTrackData>("""
            SELECT st.Id, st.Name, st.GroupDescription, s.Id as CurrentSessionId, s.SessionNumber as CurrentSessionNumber, s.Title as CurrentSessionTitle
            FROM SessionTracks as st
            LEFT JOIN (
               SELECT
                  *,
                  ROW_NUMBER() OVER (PARTITION BY SessionTrackId ORDER BY Date) as rn
               FROM Sessions
            ) s ON st.Id = s.SessionTrackId AND s.rn = 1
            ORDER BY s.Date DESC
            """)).ToList();

      var sessionTracks = new List<SessionTrackVM>();

      foreach (var data in sessionTrackDatas)
      {
         var sessionTrackVM = new SessionTrackVM(appStateService, dbContextFactory)
         {
            Id = data.Id,
            CurrentDelve = data.CurrentDelve,
            CurrentDelveId = data.CurrentDelveId,
            GroupDescription = data.GroupDescription,
            CurrentSessionId = data.CurrentSessionId,
            CurrentSessionNumber = data.CurrentSessionNumber,
            CurrentSessionTitle = data.CurrentSessionTitle,
            Name = data.Name,
         };

         sessionTracks.Add(sessionTrackVM);

         var charactersForSession = await connection.QueryAsync<int>("""
               SELECT stc.CharacterId
               FROM SessionTracksCharacters stc
               JOIN Characters c ON stc.CharacterId = c.Id
               Where stc.SessionTrackId = @SessionTrackId AND c.Status = 0
               """, new { SessionTrackId = sessionTrackVM.Id });

         foreach (var charId in charactersForSession)
         {
            var summary = characterSummaries.First(sum => sum.Id.Value == charId);

            sessionTrackVM.Characters.Add(summary);
         }
      }

      return new CampaignStateViewModel(dbContextFactory, appStateService, campaignInfo, characterSummaries, sessionTracks);
   }

   [RelayCommand]
   private async Task CreateNewSessionTrack(SessionTrack sessionTrack)
   {
      using var dbContext = dbContextFactory.CreateDbContext();

      await dbContext.SessionTracks.AddAsync(sessionTrack);

      await dbContext.SaveChangesAsync();

      var sessionTrackVM = new SessionTrackVM(appStateService, dbContextFactory)
      {
         Id = sessionTrack.Id,
         Name = sessionTrack.Name,
         GroupDescription = sessionTrack.GroupDescription,
         Characters = [.. sessionTrack.Characters.Select(stc => characterSummaries.First(cvm => cvm.Id == stc.CharacterId))]
      };

      sessionTracks.Add(sessionTrackVM);
   }

   [RelayCommand]
   private async Task CreateNewSession(SessionTrackVM? sessionTrackVM)
   {
      if (sessionTrackVM is null)
      {
         return;
      }

      using var dbContext = dbContextFactory.CreateDbContext();

      var count = await dbContext.Sessions.CountAsync(s => s.SessionTrackId == sessionTrackVM.Id);


      var sessionNumber = $"Session {count + 1}";

      var sessionTrack = await dbContext.SessionTracks
         .Include(st => st.Characters)
         .FirstAsync(st => st.Id == sessionTrackVM.Id);

      var characters = await dbContext.Characters
         .Join(dbContext.SessionTracksCharacters.Where(stc => stc.SessionTrackId == sessionTrackVM.Id), c => c.Id, stc => stc.CharacterId,
         (c, stc) => c)
         .ToListAsync();

      var session = new Session()
      {
         SessionTrack = sessionTrack,
         SessionTrackId = sessionTrack.Id,
         SessionNumber = sessionNumber,
         Title = "New Session",
         Characters = characters,
         Date = DateTime.UtcNow,
      };


      await dbContext.Sessions.AddAsync(session);
      await dbContext.SaveChangesAsync();

      var delve = await dbContext.Delves.FirstOrDefaultAsync(d => d.SessionTrackId == sessionTrackVM.Id && d.Status == DelveStatus.Active);

      if (delve is not null)
      {
         var sessionDelve = new SessionDelve()
         {
            Delve = delve,
            Session = session,
            SessionId = session.Id,
            DelveId = delve.Id,
         };

         await dbContext.SessionDelves.AddAsync(sessionDelve);
         await dbContext.SaveChangesAsync();
      }

      sessionTrackVM.CurrentSessionId = session.Id;
      sessionTrackVM.CurrentSessionNumber = session.SessionNumber;
      sessionTrackVM.CurrentSessionTitle = session.Title;

      appStateService.SetActiveSessionTrackId(sessionTrackVM.Id);
   }

   [RelayCommand]
   private void ResumeSession(SessionTrackVM sessionTrackVM)
   {
      appStateService.SetActiveSessionTrackId(sessionTrackVM.Id);
   }
}

public class CampaignInfo
{
   public string Name { get; set; } = "";
   public string SystemName { get; set; } = "";
}

public abstract class SessionTrackItem : ObservableObject { }

public sealed class NewSessionTrackVM
{

}

