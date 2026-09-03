using System;
using System.Collections.Generic;
using System.Text;
using OSRTracker.Models;

namespace OSRTracker.Data.Contracts.Services;

public interface ISessionManager
{
   Task<Session> CreateNewSession(SessionTrackId sessionTrackId, string title, CancellationToken cancellationToken = default);
}
