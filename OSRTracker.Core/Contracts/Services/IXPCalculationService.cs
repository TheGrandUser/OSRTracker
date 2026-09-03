using System;
using System.Collections.Generic;
using System.Text;
using OSRTracker.Models;

namespace OSRTracker.Contracts.Services;

public record XPApplication(Character Character, int Change);

public enum DelveCalculationMethod
{
   AnyParticipation,
   ProportionalParticipation,
   AnyAfterAcquisition,
   ProportionalAfterAcquisition,
}

public interface IXPCalculationService
{
   List<XPApplication> CalculateSessionChanges(
      List<Character> characters,
      List<GeneralXPAward> generalAwards,
      List<MonsterEntry> monsters,
      List<TreasureEntry> treasures);

   List<XPApplication> CalculateDelveChanges_AnyParticipation(
      List<Character> characters,
      List<TreasureEntry> treasures);

   List<XPApplication> CalculateDelveChanges_ProportionalParticipation(
      List<Character> characters,
      List<(SessionId, List<CharacterId>)> characterPresences,
      List<TreasureEntry> treasures);

   List<XPApplication> CalculateDelveChanges_AnyAfterAcquisition(
      List<Character> characters,
      List<(SessionId, List<CharacterId>)> characterPresences,
      List<TreasureEntry> treasures);

   List<XPApplication> CalculateDelveChanges_ProportionalAfterAcquisition(
      List<Character> characters,
      List<(SessionId, List<CharacterId>)> characterPresences,
      List<TreasureEntry> treasures);
}
