using System;
using System.Collections.Generic;
using System.Text;
using OSRTracker.Helpers;

namespace OSRTracker.Models;

public readonly record struct SessionDelveId(int Value) : IEntityId<SessionDelveId>
{
   public static SessionDelveId Empty { get; } = new(0);

   public static SessionDelveId Create(int id) => new(id);
   public override string ToString() => $"Session Delve {Value}";
}

public class SessionDelve
{
   public SessionDelveId Id { get; set; }
   public required SessionId SessionId { get; set; }
   public required DelveId DelveId { get; set; }

   public Session Session { get; set; } = null!;
   public Delve Delve { get; set; } = null!;

   public string Notes { get; set; } = string.Empty;


}
