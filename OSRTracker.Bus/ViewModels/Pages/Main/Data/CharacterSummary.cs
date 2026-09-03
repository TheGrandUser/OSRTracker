using System;
using System.Collections.Generic;
using System.Text;
using OSRTracker.Models;

namespace OSRTracker.ViewModels.Pages.Main.Data;

public class CharacterSummary
{
   public required CharacterId Id { get; set; }
   public required string Name { get; set; }
   public string? ClassName { get; set; }
   public required int Level { get; set; }
}
