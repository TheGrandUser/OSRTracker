using System;
using System.Collections.Generic;
using System.Text;

namespace OSRTracker.Core.Models;

public class Session
{
    public int Id { get; set; }
    public string SessioNumber { get; set; } = string.Empty;
    public string SessionNotes { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string? Title { get; set; }

    public List<Character> Characters { get; set; } = [];
    public List<SessionDelve> Delves { get; set; } = [];
}