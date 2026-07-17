using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using OSRTracker.Data.Contracts.Services;
using OSRTracker.Models;

namespace OSRTracker.ViewModels.Pages.GamePlay.Data;

public interface IGamePlayDataRepo
{
   Task<List<SessionTrackItem>> GetSessionTracks();
   Task<GamePlayData?> GetGamePlayDataAsync(SessionTrackId sessionTrackId);
}

internal class GamePlayDataRepo(IAppDbContextFactory dbContextFactory) : IGamePlayDataRepo
{
   public async Task<List<SessionTrackItem>> GetSessionTracks()
   {
      using var dbContext = dbContextFactory.CreateDbContext();

      await dbContext.Database.OpenConnectionAsync();

      var connection = dbContext.Database.GetDbConnection();

      var items = await connection.QueryAsync<SessionTrackItem>("""
         SELECT st.Id, st.Name
         FROM SessionTracks st
         ORDER BY st.Id
         """);

      return items.ToList();
   }

   public async Task<GamePlayData?> GetGamePlayDataAsync(SessionTrackId sessionTrackId)
   {
      using var dbContext = dbContextFactory.CreateDbContext();

      await dbContext.Database.OpenConnectionAsync();

      var connection = dbContext.Database.GetDbConnection();


      var sessionTrackDto = await connection.QueryFirstOrDefaultAsync<SessionTrackDto>("""
            SELECT st.Id, st.Name, st.GroupDescription, s.Id as SessionId, s.SessionNumber as SessionNumber, 
                   s.Title as SessionTitle, d.Id as DelveID, d.LocationName, d.LocationDescription, sd.Id as SessionDelveId, sd.Notes as SessionDelveNotes
            FROM SessionTracks as st
            LEFT JOIN Sessions s ON st.Id = s.SessionTrackId AND s.Status = 0
            LEFT JOIN Delves d ON st.Id = d.SessionTrackId AND d.Status = 0
            LEFT JOIN SessionDelves sd ON sd.SessionId = s.Id AND sd.DelveId = d.Id
            WHERE st.Id == @SessionTrackId
            LIMIT 1
            """, new { SessionTrackId = sessionTrackId });

      if (sessionTrackDto is null)
      {
         return null;
      }

      var classes = (await connection.QueryAsync<ClassDefinition>("""
         SELECT cd.Id, cd.Name, cd.Description, cd.HitDie, cd.PrimaryAbility, cd.SavingThrowProficiencies, cd.SkillProficiencies
         """)).ToList();

      var characters = (await connection.QueryAsync<CharacterDto>("""
         SELECT c.Id, c.Name, c.CurrentXP as XP, cd.Level, cd.Id as ClassId, cd.Name as ClassName, sc.CharactersId IS NOT NULL as InSession, c.XPBonus
         FROM Characters as c
         JOIN SessionTracksCharacters stc ON stc.SessionTrackId = @SessionTrackId AND stc.CharacterId = c.Id
         LEFT JOIN ClassDefinitions cd ON cd.Id = c.ClassId
         LEFT JOIN SessionCharacters sc ON sc.SessionId = @SessionId AND sc.CharactersId = c.Id
         """,
         new { SessionTrackId = sessionTrackId, sessionTrackDto.SessionId })).ToList();

      var sessionTrackData = new SessionTrackData()
      {
         Id = sessionTrackDto.Id,
         Name = sessionTrackDto.Name,
         GroupDescription = sessionTrackDto.GroupDescription ?? string.Empty
      };

      SessionData? sessionData = null;
      DelveData? delveData = null;
      SessionDelveData? sessionDelveData = null;

      List<MonsterEntry> monsters = [];
      List<GeneralXPAward> generalXPAwards = [];
      List<TreasureEntryDto> treasure = [];

      if (sessionTrackDto.HasSession)
      {
         sessionData = new SessionData()
         {
            SessionId = sessionTrackDto.SessionId.Value,
            SessionNumber = sessionTrackDto.SessionNumber ?? string.Empty,
            SessionNotes = sessionTrackDto.SessionNotes ?? string.Empty,
            SessionTitle = sessionTrackDto.SessionTitle ?? string.Empty
         };

         var monstersQuery = await connection.QueryAsync<MonsterEntry>("""
            SELECT me.Id, me.Name, me.Notes, me.Quantity, me.XPValue, me.DelveId
            FROM MonsterEntries as me
            WHERE me.SessionId = @SessionId
            """, new { SessionId = sessionTrackDto.SessionId.Value });

         foreach (var me in monstersQuery)
         {
            monsters.Add(me);
         }

         var generalXPAwardsQuery = await connection.QueryAsync<GeneralXPAward>("""
            SELECT g.Id, g.Amount, g.Description, g.DelveId
            FROM GeneralXPAwards as g
            WHERE g.SessionId = @SessionId
            """, new { SessionId = sessionTrackDto.SessionId.Value });

         foreach (var ga in generalXPAwardsQuery)
         {
            generalXPAwards.Add(ga);
         }

         var treasureQuery = await connection.QueryAsync<TreasureEntryDto>("""
            SELECT te.Id, te.ApparentValue, te.Description, te.LocType as LocationType, te.LocCharacterId, te.LocStore, 
            	    te.MagicItemDetails_IdentificationStatus as IsMagicItemIdentified, te.MagicItemDetails_TrueValue MagicItemTrueValue
            	    te.Notes, te.Quantiy as Quantity, te.Weight, te.DelveId
            FROM TreasureEntries as te
            WHERE te.SessionId = 1
            """, new { SessionId = sessionTrackDto.SessionId.Value });

         foreach (var te in treasureQuery)
         {
            treasure.Add(te);
         }
      }


      if (sessionTrackDto.HasDelve)
      {
         delveData = new DelveData()
         {
            DelveId = sessionTrackDto.DelveId.Value,
            LocationName = sessionTrackDto.LocationName ?? string.Empty,
            LocationDescription = sessionTrackDto.LocationDescription ?? string.Empty
         };
      }

      if (sessionTrackDto.HasSessionDelve)
      {
         sessionDelveData = new SessionDelveData()
         {
            SessionDelveId = sessionTrackDto.SessionDelveId.Value,
            Notes = sessionTrackDto.SessionDelveNotes ?? string.Empty
         };
      }


      var data = new GamePlayData()
      {
         SessionTrack = sessionTrackData,
         Session = sessionData,
         Delve = delveData,
         SessionDelve = sessionDelveData,
         Characters = characters,
         ClassDefinitions = classes,

         GeneralXPAwards = generalXPAwards,
         MonsterEntries = monsters,
         TreasureEntries = treasure,
      };

      return data;
   }

}
