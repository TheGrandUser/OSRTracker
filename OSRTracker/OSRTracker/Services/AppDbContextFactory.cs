using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OSRTracker.Data;
using OSRTracker.Data.Contracts.Services;

namespace OSRTracker.Services;


internal class AppDbContextFactory : IAppDbContextFactory
{
   private readonly IDbContextFactory<AppDbContext> innerFactory;
   string? connectionString;

   public bool HasPath { get; private set; }

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

      this.HasPath = true;
   }
}


internal class AppDbContextFactory2 : IAppDbContextFactory
{
   string? connectionString;
   private readonly ILoggerFactory loggerFactory;

   public bool HasPath { get; private set; }

   public AppDbContextFactory2(ILoggerFactory loggerFactory)
   {
      this.loggerFactory = loggerFactory;
   }

   public AppDbContext CreateDbContext()
   {
      if (string.IsNullOrEmpty(connectionString)) { throw new InvalidOperationException("The database file has not been set."); }

      var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

      optionsBuilder.UseSqlite(connectionString, sqliteOptions =>
      {
         sqliteOptions.MigrationsAssembly(typeof(AppDbContext).Assembly);

         // Add other common options here:
         // sqliteOptions.EnableRetryOnFailure(); // if needed
      })
         .UseModel(OSRTracker.Data.CompiledModels.AppDbContextModel.Instance)
#if DEBUG
         //.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information)
         .UseLoggerFactory(loggerFactory)
         .EnableSensitiveDataLogging()
         .EnableDetailedErrors(Debugger.IsAttached)
#endif

         ;


      return new AppDbContext(optionsBuilder.Options);
   }

   public void SetDbPath(string filePath)
   {
      this.connectionString = $"Data source={filePath};Pooling=True;Cache=Shared";

      this.HasPath = true;
   }
}
