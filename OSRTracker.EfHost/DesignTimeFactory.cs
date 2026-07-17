using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using OSRTracker.Data;

namespace OSRTracker.EfHost;

public class DesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
   private readonly DbContextOptions<AppDbContext> options;

   public DesignTimeFactory()
   {
      var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

      optionsBuilder.UseSqlite("Data Source=designtime.db",
         ob =>
         {
            ob.MigrationsAssembly(typeof(AppDbContext).Assembly);
         });

      options = optionsBuilder.Options;
   }

   public AppDbContext CreateDbContext(string[] args)
   {
      return new AppDbContext(options);
   }
}
