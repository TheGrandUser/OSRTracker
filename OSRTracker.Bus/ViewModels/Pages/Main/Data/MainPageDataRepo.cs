using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Microsoft.EntityFrameworkCore;
using OSRTracker.Data.Contracts.Services;

namespace OSRTracker.ViewModels.Pages.Main.Data;

public interface IMainPageDataRepo
{
   Task<CampaignData> GetCampaignsAsync();

}

public class MainPageDataRepo(IAppDbContextFactory dbContextFactory) : IMainPageDataRepo
{
   public async Task<CampaignData> GetCampaignsAsync()
   {

      using var dbContext = await dbContextFactory.CreateDbContextAsync();

      await dbContext.Database.OpenConnectionAsync();

      var connection = dbContext.Database.GetDbConnection();

      var campaignInfo = await connection.QueryFirstAsync<CampaignInfo>("""
            SELECT c.Name, c.SystemName
            FROM CampaignSettings c
            WHERE c.Id = 1
            """);

      var characters = (await connection.QueryAsync<CharacterSummary>("""
            SELECT c.Id, c.Name, IfNull(cd.Name, "None") AS ClassName, c.Level
            FROM Characters as c
            LEFT JOIN ClassDefinitions AS cd
            ON cd.Id = c.ClassId
            WHERE c.Status = 0 AND c.CharacterType IN (0, 1)
            ORDER BY c.Name ASC
            """)).ToList();

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

      foreach (var data in sessionTrackDatas)
      {
         var charactersForSession = await connection.QueryAsync<int>("""
               SELECT stc.CharacterId
               FROM SessionTracksCharacters stc
               JOIN Characters c ON stc.CharacterId = c.Id
               Where stc.SessionTrackId = @SessionTrackId AND c.Status = 0
               """, new { SessionTrackId = data.Id });

         data.Characters = charactersForSession.ToList();
      }

      return new CampaignData()
      {
         CampaignInfo = campaignInfo,
         Characters = characters,
         SessionTracks = sessionTrackDatas
      };
   }
}
