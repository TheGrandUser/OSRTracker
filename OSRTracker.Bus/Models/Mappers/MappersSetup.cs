using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using OSRTracker.Models;
using OSRTracker.Models.Mappers;

namespace Microsoft.Extensions.DependencyInjection;

public static class MappersSetup
{
   extension(IServiceCollection self)
   {
      public void AddMappers()
      {
         SqlMapper.AddTypeHandler(new JsonTypeHandler<List<LevelXPRequirement>>());
      }
   }
}
