using System;
using System.Collections.Generic;
using System.Text;
using OSRTracker.Helpers;

namespace OSRTracker.Models;

public readonly record struct SessionId(int Id) : IEntityId<SessionId>
{
   public static SessionId Empty { get; } = new(0);

   public static SessionId Create(int id) => new(id);
   public override string ToString() => $"Session {Id}";
}

public class Session
{
   public SessionId Id { get; set; }

   public required SessionTrackId SessionTrackId { get; set; }
   public SessionTrack? SessionTrack { get; set; }


   public string SessionNumber { get; set; } = string.Empty;
   public string SessionNotes { get; set; } = string.Empty;

   public SessionStatus Status { get; set; }

   public DateTime Date { get; set; }
   public string? Title { get; set; }

   public List<Character> Characters { get; set; } = [];
   public List<SessionDelve> Delves { get; set; } = [];
}

public enum SessionStatus
{
   Active = 0,
   Finished,
}