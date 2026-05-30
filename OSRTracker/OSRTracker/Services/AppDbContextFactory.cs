using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using OSRTracker.Data;
using OSRTracker.Data.Contracts.Services;

namespace OSRTracker.Services;


internal class AppDbContextFactory : IAppDbContextFactory
{
   private readonly IDbContextFactory<AppDbContext> innerFactory;
   string? connectionString;

   public AppDbContextFactory(IDbContextFactory<AppDbContext> innerFactory)
   {
      this.innerFactory = innerFactory;
   }

   public AppDbContext CreateDbContext()
   {
      if (string.IsNullOrEmpty(connectionString)) { throw new InvalidOperationException("The database file has not been set."); }

      var dbContext = innerFactory.CreateDbContext();

      dbContext.Database.SetConnectionString(connectionString);

      return dbContext;
   }

   public void SetDbPath(string filePath)
   {
      this.connectionString = $"Data source={filePath}";
   }
}
