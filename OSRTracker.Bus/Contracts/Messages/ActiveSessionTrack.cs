using System;
using System.Collections.Generic;
using System.Text;
using OSRTracker.Models;

namespace OSRTracker.Contracts.Messages;

public class ActiveSessionTrack(SessionTrackId? id)
{
   public SessionTrackId? SessionId { get; } = id;
}
