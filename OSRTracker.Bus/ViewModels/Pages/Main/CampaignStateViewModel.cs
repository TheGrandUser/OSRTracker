using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using OSRTracker.Contracts.Messages;
using OSRTracker.Contracts.Services;
using OSRTracker.Data.Contracts.Services;
using OSRTracker.Models;
using OSRTracker.ViewModels.Pages.Main.Data;

namespace OSRTracker.ViewModels.Pages.Main;

public partial class CampaignStateViewModel : MainStateViewModel
{
   private readonly IAppDbContextFactory dbContextFactory;
   private readonly IAppStateService appStateService;
   private readonly ISessionManager sessionManager;
   private readonly ITimeSource timeSource;

   private readonly ObservableCollection<SessionTrackVM> sessionTracks = [];
   private readonly ObservableCollection<CharacterSummaryVM> characterSummaries = [];

   private SessionTrackVM? activeTrack;

   public CampaignStateViewModel(
      IAppDbContextFactory dbContextFactory,
      IAppStateService appStateService,
      ISessionManager sessionManager,
      ITimeSource timeSource,
      CampaignInfo campaignInfo,
      List<CharacterSummaryVM> characters,
      List<SessionTrackVM> sessionTracks)
   {
      this.dbContextFactory = dbContextFactory;
      this.appStateService = appStateService;
      this.sessionManager = sessionManager;
      this.timeSource = timeSource;

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
         st.IsActiveSession = st.Id == appStateService.ActiveSessionTrackId;
         if (st.IsActiveSession)
         {
            activeTrack = st;
         }
         this.sessionTracks.Add(st);
      }

      WeakReferenceMessenger.Default.Register<ActiveSessionTrack>(this, OnActiveSessionChanged);
   }

   public static async Task<CampaignStateViewModel> CreateAsync(IServiceProvider serviceProvider)
   {
      var mainPageDataRepo = serviceProvider.GetRequiredService<IMainPageDataRepo>();

      var campaignData = await mainPageDataRepo.GetCampaignsAsync();

      var campaignInfo = campaignData.CampaignInfo;

      List<CharacterSummaryVM> characterSummaries = [.. campaignData.Characters.Select(cs => new CharacterSummaryVM() { Id = cs.Id, Level = cs.Level, Name = cs.Name, ClassName = cs.ClassName })];

      var sessionTracks = new List<SessionTrackVM>();

      var dbContextFactory = serviceProvider.GetRequiredService<IAppDbContextFactory>();

      foreach (var data in campaignData.SessionTracks)
      {
         var sessionTrackVM = new SessionTrackVM(dbContextFactory)
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

         foreach (var charId in data.Characters)
         {
            var summary = characterSummaries.First(sum => sum.Id.Value == charId);

            sessionTrackVM.Characters.Add(summary);
         }
      }

      return ActivatorUtilities.CreateInstance<CampaignStateViewModel>(serviceProvider, campaignInfo, characterSummaries, sessionTracks);
   }

   private void OnActiveSessionChanged(object recipient, ActiveSessionTrack message)
   {
      foreach (var vm in sessionTracks)
      {
         vm.OnActiveSessionChanged(message.SessionId);
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

   [RelayCommand]
   private async Task CreateNewSessionTrack(SessionTrack sessionTrack)
   {
      using var dbContext = dbContextFactory.CreateDbContext();

      await dbContext.SessionTracks.AddAsync(sessionTrack);

      await dbContext.SaveChangesAsync();

      var sessionTrackVM = new SessionTrackVM(dbContextFactory)
      {
         Id = sessionTrack.Id,
         Name = sessionTrack.Name,
         GroupDescription = sessionTrack.GroupDescription,
         Characters = [.. sessionTrack.Characters.Select(stc => characterSummaries.First(cvm => cvm.Id == stc.CharacterId))],
         IsActiveSession = sessionTrack.Id == appStateService.ActiveSessionTrackId,
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

      var session = await sessionManager.CreateNewSession(sessionTrackVM.Id, "New Session");

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

