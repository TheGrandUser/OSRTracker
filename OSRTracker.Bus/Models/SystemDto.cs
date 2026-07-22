using System;
using System.Collections.Generic;
using System.Text;
using OSRTracker.Models;

namespace OSRTracker.Models;

public class SystemDto
{
   public string SystemName { get; set; } = string.Empty;
   public List<string> Attributes { get; set; } = [];
   public List<ClassDto> Classes { get; set; } = [];
}

public class ClassDto
{
   public string Name { get; set; } = string.Empty;
   public List<int> LevelXP { get; set; } = [];
   public List<string> KeyAttributes { get; set; } = [];

   public static ClassDto FromClassDefinition(ClassDefinition classDef)
   {
      return new ClassDto()
      {
         Name = classDef.Name,
         KeyAttributes = classDef.KeyAttributes.Select(a => a.Name).ToList(),
         LevelXP = classDef.LevelXP.Select(l => l.XP).ToList()
      };
   }

   public ClassDefinition ToClassDefinition(Dictionary<string, AttributeDefinition> attributeDefinitions)
   {

      var classDef = new ClassDefinition()
      {
         Name = Name
      };
      foreach (var attr in KeyAttributes)
      {
         classDef.KeyAttributes.Add(attributeDefinitions[attr]);
      }
      foreach (var levelXP in LevelXP.Order())
      {
         classDef.LevelXP.Add(new LevelXPRequirement(levelXP));
      }


      return classDef;
   }
}


