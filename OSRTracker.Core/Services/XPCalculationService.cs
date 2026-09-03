using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using OSRTracker.Contracts.Services;
using OSRTracker.Models;

namespace OSRTracker.Services;

public class XPCalculationService : IXPCalculationService
{
   //throw new InvalidOperationException($"Could not find award character {c.Name} in the list of session characters");


   public List<XPApplication> CalculateSessionChanges(List<Character> characters, List<GeneralXPAward> generalAwards, List<MonsterEntry> monsters, List<TreasureEntry> treasures)
   {
      if (characters.Count == 0)
      {
         throw new ArgumentException("Must have some characters");
      }

      var totalAwardXP = generalAwards.Where(a => a.Characters.Count == 0).Sum(a => a.Amount);

      Span<int> specificXPPerCharacter = stackalloc int[characters.Count];

      foreach (var generalAward in generalAwards.Where(a => a.Characters.Count > 0))
      {
         foreach (var c in generalAward.Characters)
         {
            var index = characters.IndexOf(c);

            Debug.Assert(index > 0, "Could not find award character in session characters");

            if (index == -1)
            {
               // Report error

               continue;
            }

            specificXPPerCharacter[index] = generalAward.Amount;
         }
      }

      var totalMonsterXP = monsters.Sum(m => m.XPValue * m.Quantity);
      var totalTreasureXP = CalculateTotalTreasureXP(treasures);

      var totalXP = totalAwardXP + totalMonsterXP + totalTreasureXP;

      return CalculateApplications(characters, specificXPPerCharacter, totalXP);
   }

   public List<XPApplication> CalculateDelveChanges_AnyParticipation(List<Character> characters, List<TreasureEntry> treasures)
   {

      var totalTreasureXP = CalculateTotalTreasureXP(treasures);

      var totalXP = totalTreasureXP;

      return CalculateApplications(characters, [], totalXP);


   }

   public List<XPApplication> CalculateDelveChanges_ProportionalParticipation(List<Character> characters, List<(SessionId, List<CharacterId>)> characterPresences, List<TreasureEntry> treasures)
   {
      Span<int> countsPresent = stackalloc int[characters.Count];

      foreach (var (_, charactersInSession) in characterPresences)
      {
         for (var i = 0; i < characters.Count; i++)
         {
            var character = characters[i];

            if (charactersInSession.Contains(character.Id))
            {
               countsPresent[i]++;
            }
         }
      }

      var sessionCount = characterPresences.Count;

      Span<decimal> shares = stackalloc decimal[characters.Count];
      decimal totalShares = 0;

      for (var i = 0; i < countsPresent.Length; i++)
      {
         var character = characters[i];
         var count = countsPresent[i];

         var shareWeight = count / (decimal)sessionCount;
         var share = character.ShareMultiplierXP * shareWeight;

         shares[i] = share;
         totalShares += share;
      }

      var totalTreasureXP = CalculateTotalTreasureXP(treasures);

      var totalXP = totalTreasureXP;

      var baseXPShare = (int)Math.Ceiling(totalXP / totalShares);

      List<XPApplication> xpApplications = [];



      for (var i = 0; i < shares.Length; i++)
      {
         var character = characters[i];
         var share = shares[i];

         var adjustment = 1 + character.XPBonus;

         var baseCharacterXP = share * baseXPShare;

         var adjustedXP = (int)Math.Ceiling(baseCharacterXP * adjustment);

         xpApplications.Add(new XPApplication(character, adjustedXP));
      }

      return xpApplications;
   }

   public List<XPApplication> CalculateDelveChanges_AnyAfterAcquisition(List<Character> characters, List<(SessionId, List<CharacterId>)> characterPresences, List<TreasureEntry> treasures)
   {
      if (characters.Count == 0)
      {
         throw new ArgumentException("Must have some characters");
      }

      if (characterPresences.Count == 0)
      {
         throw new ArgumentException("Must have some sessions in the delve");
      }

      var treasuresBySession = treasures.GroupBy(t => t.SessionId).ToDictionary(g => g.Key, g => g.ToList());

      Span<decimal> baseCharacterTreasure = stackalloc decimal[characters.Count];

      decimal allTreasureSum = 0;

      var applicableCharactersBuffer = new Character[characters.Count];

      var setOfCharactersCurrentOrAfter = new HashSet<CharacterId>();
      for (var i = characterPresences.Count - 1; i >= 0; i--)
      {
         var (sessionId, characterIdsInSession) = characterPresences[i];

         setOfCharactersCurrentOrAfter.UnionWith(characterIdsInSession);

         if (!treasuresBySession.TryGetValue(sessionId, out var treasuresThisSession))
         {
            continue;
         }

         var treasureXPThisSession = CalculateTotalTreasureXP(treasuresThisSession);

         if (setOfCharactersCurrentOrAfter.Count == characters.Count)
         {
            allTreasureSum += treasureXPThisSession;
         }
         else
         {
            var applicableCharacters = applicableCharactersBuffer.AsSpan(setOfCharactersCurrentOrAfter.Count);
            var j = 0;
            foreach (var id in setOfCharactersCurrentOrAfter)
            {
               applicableCharacters[j] = characters.First(c => c.Id == id);
               j++;
            }

            var totalShares = applicableCharacters.Sum(c => c.ShareMultiplierXP);

            var xpPerShare = treasureXPThisSession / totalShares;

            foreach (var c in applicableCharacters)
            {
               var charIndex = characters.IndexOf(c);

               baseCharacterTreasure[charIndex] += xpPerShare * c.ShareMultiplierXP;
            }
         }
      }

      Span<int> baseCharacterXP = stackalloc int[baseCharacterTreasure.Length];

      for (var i = 0; i < baseCharacterXP.Length; i++)
      {
         baseCharacterXP[i] = (int)Math.Ceiling(baseCharacterTreasure[i]);
      }

      return CalculateApplications(characters, baseCharacterXP, (int)Math.Ceiling(allTreasureSum));
   }

   public List<XPApplication> CalculateDelveChanges_ProportionalAfterAcquisition(List<Character> characters, List<(SessionId, List<CharacterId>)> characterPresences, List<TreasureEntry> treasures)
   {
      if (characters.Count == 0)
      {
         throw new ArgumentException("Must have some characters");
      }

      if (characterPresences.Count == 0)
      {
         throw new ArgumentException("Must have some sessions in the delve");
      }

      Span<int> countsPresent = stackalloc int[characters.Count];
      Span<decimal> characterTotalTreasureXP = stackalloc decimal[characters.Count];

      Span<decimal> shares = stackalloc decimal[characters.Count];

      var treasuresBySession = treasures.GroupBy(t => t.SessionId).ToDictionary(g => g.Key, g => g.ToList());

      var sessionCount = 0;

      for (var sessionIndex = characterPresences.Count - 1; sessionIndex >= 0; sessionIndex--)
      {
         var (sessionId, characterIdsInSession) = characterPresences[sessionIndex];

         sessionCount++;

         for (var j = 0; j < characters.Count; j++)
         {
            var character = characters[j];

            if (characterIdsInSession.Contains(character.Id))
            {
               countsPresent[j]++;
            }
         }

         if (!treasuresBySession.TryGetValue(sessionId, out var treasuresThisSession))
         {
            continue;
         }

         decimal totalShares = 0;

         for (var j = 0; j < countsPresent.Length; j++)
         {
            var character = characters[j];
            var count = countsPresent[j];

            if (count == 0)
            {
               shares[j] = 0;
               continue;
            }

            var shareWeight = count / (decimal)sessionCount;
            var share = character.ShareMultiplierXP * shareWeight;

            shares[j] = share;
            totalShares += share;
         }


         var treasureXPThisSession = CalculateTotalTreasureXP(treasuresThisSession);

         var baseXPShare = (int)Math.Ceiling(treasureXPThisSession / totalShares);

         for (var i = 0; i < shares.Length; i++)
         {
            var share = shares[i];

            var baseCharacterXP = share * baseXPShare;

            characterTotalTreasureXP[i] += baseCharacterXP;
         }
      }


      {
         Span<int> characterTotalTreasureXPRounded = stackalloc int[characterTotalTreasureXP.Length];

         for (var i = 0; i < characterTotalTreasureXPRounded.Length; i++)
         {
            characterTotalTreasureXPRounded[i] = (int)Math.Ceiling(characterTotalTreasureXP[i]);
         }

         return CalculateApplications(characters, characterTotalTreasureXPRounded, 0);
      }
   }

   private static int CalculateTotalTreasureXP(List<TreasureEntry> treasures)
   {
      var sum = treasures.Sum(t =>
      {
         if (t.MagicItemDetails is null)
         {
            return t.Value * t.Quantiy;
         }
         else
         {
            var apparentValue = t.MagicItemDetails.ApparentValue;
            var trueValue = t.Value;
            var value = (t.SaleStatus, t.MagicItemDetails.IdentificationStatus) switch
            {
               (TreasureSale.SoldWithoutUse, IdentificationStatus.PartiallyIdentified) => trueValue,
               (TreasureSale.SoldWithoutUse, IdentificationStatus.FullyIdentified) => trueValue,

               _ => apparentValue,
            };

            return value * t.Quantiy;

         }
      });

      return (int)Math.Round(sum);
   }

   private static List<XPApplication> CalculateApplications(List<Character> characters, Span<int> specificXPPerCharacter, int totalXP)
   {
      List<XPApplication> xpApplications = [];

      var totalShares = characters.Sum(c => c.ShareMultiplierXP);

      if (totalShares == 0)
      {
         throw new ArgumentException("Characters have no shares among them");
      }

      var baseXPShare = (int)Math.Ceiling(totalXP / totalShares);

      for (var i = 0; i < specificXPPerCharacter.Length; i++)
      {
         var character = characters[i];
         var specificXP = specificXPPerCharacter[i];

         var adjustment = 1 + character.XPBonus;

         var baseCharacterXP = character.ShareMultiplierXP * baseXPShare;

         var adjustedXP = (int)Math.Ceiling((baseCharacterXP + specificXP) * adjustment);

         xpApplications.Add(new XPApplication(character, adjustedXP));
      }

      return xpApplications;
   }

}
