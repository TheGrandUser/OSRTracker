using System;
using System.Collections.Generic;
using System.Text;
using OSRTracker.Helpers;

namespace OSRTracker.Models;

public readonly record struct SessionTrackId(int Id) : IEntityId<SessionTrackId>
{
   public static SessionTrackId Empty { get; } = new(0);

   public static SessionTrackId Create(int id) => new(id);
   public override string ToString() => $"Session Track {Id}";
}

public class SessionTrack
{
   public SessionTrackId Id { get; set; }

   public required string Name { get; set; }

   public string? GroupDescription { get; set; }

   public List<SessionTrackCharacter> Characters { get; set; } = [];

   public List<Session> Sessions { get; set; } = [];
   public List<Delve> Delves { get; set; } = [];
}

public class SessionTrackCharacter
{
   public required SessionTrackId SessionTrackId { get; set; }
   public SessionTrack? SessionTrack { get; set; }
   public required CharacterId CharacterId { get; set; }
   public Character? Character { get; set; }
}