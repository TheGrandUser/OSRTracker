using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSRTracker.Services;
using OSRTracker.Models;
using OSRTracker.Contracts.Services;

namespace OSRTracker.Core.UnitTests.Services
{
   [TestClass]
   public class XPCalculationServiceTests
   {
      // Helper to create a list of characters (party)
      private static List<Character> CreateParty(int count)
      {
         var list = new List<Character>(count);
         for (int i = 0; i < count; i++)
         {
            list.Add(new Character
            {
               Id = CharacterId.Create(i + 1),
               Name = $"Char{i + 1}",
               XPBonus = 0m
            });
         }

         return list;
      }

      private static Session CreateSession(List<Character> characters)
      {
         Session session = new() { SessionTrackId = SessionTrackId.Create(1), };

         session.Characters = characters.Select(c => new SessionCharacter { CharacterId = c.Id, Character = c, SessionId = session.Id, Session = session }).ToList();

         return session;
      }

      [TestMethod]
      public void CalculateSessionChanges_NoSpecificAwards_ComputesEvenShareAndAppliesNoBonus()
      {
         // Arrange
         var svc = new XPCalculationService();
         var characters = CreateParty(2);
         var session = CreateSession(characters);

         // One general award that applies to all (Characters.Count == 0)
         var generalAwards = new List<GeneralXPAward>
            {
                new GeneralXPAward { Amount = 0, Session = session }
            };

         var monsters = new List<MonsterEntry>
            {
                new MonsterEntry { Name = "Goblin", Quantity = 1, XPValue = 100, Session = session }
            };

         var treasures = new List<TreasureEntry>
            {
                new TreasureEntry { Quantiy = 2, Value = 50m, Session = session }
            };

         // total XP = 0 (awards) + 100 (monsters) + 2*50 = 200
         // per character = ceil(200 / 2) = 100

         // Act
         var result = svc.CalculateSessionChanges(characters, generalAwards, monsters, treasures);

         // Assert
         Assert.IsNotNull(result);
         Assert.AreEqual(2, result.Count);
         Assert.AreEqual(100, result[0].Change);
         Assert.AreEqual(100, result[1].Change);
      }

      [TestMethod]
      public void CalculateSessionChanges_SpecificAwardsAndMagicItems_UsesApparentAndFullyIdentifiedValuesAndAppliesBonus()
      {
         // Arrange
         var svc = new XPCalculationService();
         var characters = CreateParty(2);
         var session = CreateSession(characters);

         // Give the second character a small bonus
         characters[1].XPBonus = 0.10m; // +10%

         // One specific award for character index 1
         var specificAward = new GeneralXPAward { Amount = 20, Characters = new List<Character> { characters[1] }, Session = session };

         // One award that applies to no specific character
         var generalAwards = new List<GeneralXPAward>
            {
                new GeneralXPAward { Amount = 10, Session = session },
                specificAward
            };

         var monsters = new List<MonsterEntry>
            {
                new MonsterEntry { Name = "Orc", Quantity = 2, XPValue = 30, Session = session } // 60 XP
            };

         var treasures = new List<TreasureEntry>
            {
                // Magic item unidentified: uses ApparentValue (30)
                new TreasureEntry { Quantiy = 1, Value = 100m, MagicItemDetails = new MagicItemDetails(30m, IdentificationStatus.Unidentified), Session = session },
                // Magic item fully identified: uses Value (200)
                new TreasureEntry { Quantiy = 1, Value = 200m, MagicItemDetails = new MagicItemDetails(50m, IdentificationStatus.FullyIdentified), Session = session }
            };

         // Compute expected
         // totalAwardXP = 10 (general award with no characters)
         // totalMonsterXP = 60
         // totalTreasureXP = 30 (apparent) + 200 (value when fully identified) = 230
         // totalXP = 10 + 60 + 230 = 300
         // base per character = ceil(300 / 2) = 150
         // specificXP for char0 = 0, char1 = 20
         // adjusted char0 = ceil((150 + 0) * (1 + 0)) = 150
         // adjusted char1 = ceil((150 + 20) * (1 + 0.10)) = ceil(170 * 1.1) = ceil(187) = 187

         // Act
         var result = svc.CalculateSessionChanges(characters, generalAwards, monsters, treasures);

         // Assert
         Assert.IsNotNull(result);
         Assert.AreEqual(2, result.Count);
         Assert.AreEqual(150, result[0].Change);
         Assert.AreEqual(187, result[1].Change);
      }
   }
}
