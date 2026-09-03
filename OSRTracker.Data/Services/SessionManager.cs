using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using OSRTracker.Contracts.Services;
using OSRTracker.Data.Contracts.Services;
using OSRTracker.Models;

namespace OSRTracker.Data.Services;

public class SessionManager(IAppDbContextFactory dbContextFactory, ITimeSource timeSource) : ISessionManager
{
   public async Task<Session> CreateNewSession(SessionTrackId sessionTrackId, string title, CancellationToken cancellationToken = default)
   {
      using var dbContext = dbContextFactory.CreateDbContext();

      var count = await dbContext.Sessions.CountAsync(s => s.SessionTrackId == sessionTrackId, cancellationToken);


      var sessionNumber = $"Session {count + 1}";

      var sessionTrack = await dbContext.SessionTracks
         .Include(st => st.Characters)
         .FirstAsync(st => st.Id == sessionTrackId, cancellationToken);

      var characters = await dbContext.Characters
         .Join(dbContext.SessionTracksCharacters.Where(stc => stc.SessionTrackId == sessionTrackId), c => c.Id, stc => stc.CharacterId,
         (c, stc) => c)
         .ToListAsync(cancellationToken);



      var session = new Session()
      {
         SessionTrack = sessionTrack,
         SessionTrackId = sessionTrack.Id,
         SessionNumber = sessionNumber,
         Title = title,
         Date = timeSource.GetUtcNow(),
      };


      await dbContext.Sessions.AddAsync(session, cancellationToken);
      await dbContext.SaveChangesAsync(cancellationToken);

      session.Characters = [.. characters.Select(c => new SessionCharacter { CharacterId = c.Id, Character = c, SessionId = session.Id, Session = session })];

      var delve = await dbContext.Delves.FirstOrDefaultAsync(d => d.SessionTrackId == sessionTrackId && d.Status == DelveStatus.Active, cancellationToken);

      if (delve is not null)
      {
         var sessionDelve = new SessionDelve()
         {
            Delve = delve,
            Session = session,
            SessionId = session.Id,
            DelveId = delve.Id,
         };

         await dbContext.SessionDelves.AddAsync(sessionDelve, cancellationToken);
      }

      await dbContext.SaveChangesAsync(cancellationToken);

      return session;
   }
}
