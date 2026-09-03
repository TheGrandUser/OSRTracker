using Microsoft.EntityFrameworkCore;
using OSRTracker.Models;

namespace OSRTracker.EfHost;

internal class Program
{
   static void Main(string[] args)
   {
      Console.WriteLine("Hello, World!");
   }

   static void Test(ModelBuilder modelBuilder)
   {
      modelBuilder.Entity<AttributeDefinition>(entity =>
      {
         entity.HasKey(x => x.Id);
         entity.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new AttributeDefinitionId(id))
            .HasColumnType("INTEGER")
            .ValueGeneratedOnAdd()
            .UseAutoincrement()
            .HasAnnotation("Sqlite:Autoincrement", true);
      });
   }
}
