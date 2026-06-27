using System;
using System.Collections.Generic;
using System.Text;
using OSRTracker.Models;

namespace OSRTracker.Contracts.Messages;

internal class ActiveSession(SessionTrackId? id)
{
   public SessionTrackId? SessionId { get; } = id;
}
