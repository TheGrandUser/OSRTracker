using System;
using System.Collections.Generic;
using System.Text;
using OSRTracker.Models;

namespace OSRTracker.ViewModels.Pages.Main.Data;


public sealed class SessionTrackData
{
   public SessionTrackId Id { get; set; }
   public string Name { get; set; } = string.Empty;
   public string? GroupDescription { get; set; }
   public SessionId? CurrentSessionId { get; set; }
   public string? CurrentSessionNumber { get; set; }
   public string? CurrentSessionTitle { get; set; }


   public bool HasDelve => CurrentDelveId.HasValue;

   public DelveId? CurrentDelveId { get; set; }
   public string? CurrentDelve { get; set; }
   public List<int> Characters { get; set; } = [];
}
