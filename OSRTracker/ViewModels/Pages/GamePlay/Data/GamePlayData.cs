using System;
using System.Collections.Generic;
using System.Text;
using OSRTracker.Models;

namespace OSRTracker.ViewModels.Pages.GamePlay.Data;

public class GamePlayData
{
   public required SessionTrackData SessionTrack { get; init; }
   public SessionData? Session { get; init; }
   public DelveData? Delve { get; init; }
   public SessionDelveData? SessionDelve { get; init; }
   public required List<CharacterDto> Characters { get; init; }
   public required List<ClassDefinition> ClassDefinitions { get; init; }
   public required List<GeneralXPAward> GeneralXPAwards { get; init; }
   public required List<MonsterEntry> MonsterEntries { get; init; }
   public required List<TreasureEntryDto> TreasureEntries { get; init; }
}

public class SessionTrackItem
{
   public required SessionTrackId Id { get; set; }
   public required string Name { get; set; }
}

public class SessionTrackData
{
   public required SessionTrackId Id { get; set; }
   public required string Name { get; set; }
   public required string GroupDescription { get; set; }
}

public class  SessionData
{
   public required SessionId SessionId { get; set; }
   public required string SessionNumber { get; set; }
   public required string SessionNotes { get; set; }
   public required string SessionTitle { get; set; }
}

public class DelveData
{
   public required DelveId DelveId { get; set; }
   public required string LocationDescription { get; set; }
   public required string LocationName { get; set; }
}

public class SessionDelveData
{
   public required SessionDelveId SessionDelveId { get; set; }
   public required string Notes { get; set; }
}