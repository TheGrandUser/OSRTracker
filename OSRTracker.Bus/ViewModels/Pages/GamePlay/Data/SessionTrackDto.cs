using System.Diagnostics.CodeAnalysis;
using OSRTracker.Models;

namespace OSRTracker.ViewModels.Pages.GamePlay.Data;

public class SessionTrackDto
{
   public SessionTrackId Id { get; set; }
   public string Name { get; set; } = string.Empty;
   public string? GroupDescription { get; set; }

   [MemberNotNullWhen(true, nameof(SessionId))]
   [MemberNotNullWhen(true, nameof(SessionNumber))]
   [MemberNotNullWhen(true, nameof(SessionNotes))]
   [MemberNotNullWhen(true, nameof(SessionTitle))]
   public bool HasSession => SessionId.GetValueOrDefault().Value != 0;
   public SessionId? SessionId { get; set; }
   public string? SessionNumber { get; set; }
   public string? SessionNotes { get; set; }
   public string? SessionTitle { get; set; }

   [MemberNotNullWhen(true, nameof(DelveId))]
   [MemberNotNullWhen(true, nameof(LocationDescription))]
   [MemberNotNullWhen(true, nameof(LocationName))]
   public bool HasDelve => DelveId.GetValueOrDefault().Value != 0;
   public DelveId? DelveId { get; set; }
   public string? LocationDescription { get; set; }
   public string? LocationName { get; set; }

   [MemberNotNullWhen(true, nameof(SessionId))]
   [MemberNotNullWhen(true, nameof(SessionNumber))]
   [MemberNotNullWhen(true, nameof(SessionNotes))]
   [MemberNotNullWhen(true, nameof(SessionTitle))]
   [MemberNotNullWhen(true, nameof(DelveId))]
   [MemberNotNullWhen(true, nameof(LocationDescription))]
   [MemberNotNullWhen(true, nameof(LocationName))]
   [MemberNotNullWhen(true, nameof(SessionDelveId))]
   [MemberNotNullWhen(true, nameof(SessionDelveNotes))]
   public bool HasSessionDelve => SessionDelveId.HasValue;
   public SessionDelveId? SessionDelveId { get; set; }
   public string? SessionDelveNotes { get; set; } = string.Empty;
}
