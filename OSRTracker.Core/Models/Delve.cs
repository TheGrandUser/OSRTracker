using System;
using System.Collections.Generic;
using System.Text;
using OSRTracker.Helpers;

namespace OSRTracker.Models;

public readonly record struct DelveId(int Id) : IEntityId<DelveId>
{
   public static DelveId Empty { get; } = new(0);
   public static DelveId Create(int id) => new(id);
   public override string ToString() => $"Delve {Id}";
}

public class Delve
{
   public DelveId Id { get; set; }
   public required string LocationName { get; set; }

   public string LocationDescription { get; set; } = string.Empty;

   public DelveStatus Status { get; set; }

   public List<SessionDelve> Sessions { get; set; } = [];

   public SessionTrack? SessionTrack { get; set; }
   public required SessionTrackId SessionTrackId { get; set; }
}

public enum DelveStatus
{
   Active,
   Completed,
}
