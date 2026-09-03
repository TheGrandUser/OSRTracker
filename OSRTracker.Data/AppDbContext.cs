using Microsoft.EntityFrameworkCore;
using OSRTracker.Models;

namespace OSRTracker.Data;

public class AppDbContext : DbContext
{
   public AppDbContext()
   {

   }

   public AppDbContext(DbContextOptions<AppDbContext> options)
      : base(options)
   {

   }

   private static readonly CampaignId Sole = new(1);

   private static readonly Func<AppDbContext, Task<CampaignSettings?>> _getCampaign =
       EF.CompileAsyncQuery((AppDbContext ctx) =>
       ctx.CampaignSettings.AsNoTracking().FirstOrDefault(x => x.Id == Sole));

   public async Task<CampaignSettings> GetCampaignAsync()
   {
      return await _getCampaign(this) ?? throw new InvalidOperationException("Campaign not found");
   }

   public DbSet<AttributeDefinition> AttributeDefinitions { get; set; }
   public DbSet<CampaignSettings> CampaignSettings { get; set; }
   public DbSet<Character> Characters { get; set; }
   public DbSet<ClassDefinition> ClassDefinitions { get; set; }
   public DbSet<CurrencyDefinition> CurrencyDefinitions { get; set; }
   public DbSet<Delve> Delves { get; set; }
   public DbSet<GeneralXPAward> GeneralXPAwards { get; set; }
   public DbSet<MonsterEntry> MonsterEntries { get; set; }
   public DbSet<Session> Sessions { get; set; }
   public DbSet<SessionCharacter> SessionCharacters { get; set; }
   public DbSet<SessionDelve> SessionDelves { get; set; }
   public DbSet<SessionTrack> SessionTracks { get; set; }
   public DbSet<SessionTrackCharacter> SessionTracksCharacters { get; set; }
   public DbSet<TreasureEntry> TreasureEntries { get; set; }


   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<AttributeDefinition>(entity =>
      {
         entity.HasKey(x => x.Id);
         entity.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new AttributeDefinitionId(id))
            .HasColumnType("INTEGER")
            .ValueGeneratedOnAdd()
            .UseAutoincrement();
         //.HasAnnotation("Sqlite:Autoincrement", true);
      });

      modelBuilder.Entity<CampaignSettings>(entity =>
      {
         entity.HasKey(x => x.Id);
         entity.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new CampaignId(id))
            .HasColumnType("INTEGER")
            .ValueGeneratedOnAdd()
            .UseAutoincrement();
      });

      modelBuilder.Entity<Character>(entity =>
      {
         entity.HasKey(x => x.Id);
         entity.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new CharacterId(id))
            .HasColumnType("INTEGER")
            .ValueGeneratedOnAdd()
            .UseAutoincrement();
         //.HasAnnotation("Sqlite:Autoincrement", true);

         entity.HasOne(x => x.Class)
               .WithMany()
               .HasForeignKey(a => a.ClassId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);
      });

      modelBuilder.Entity<ClassDefinition>(entity =>
      {
         entity.HasKey(x => x.Id);
         entity.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new ClassDefinitionId(id))
            .HasColumnType("INTEGER")
            .ValueGeneratedOnAdd()
            .UseAutoincrement();
         //.HasAnnotation("Sqlite:Autoincrement", true);

         entity.OwnsMany(x => x.LevelXP, owner =>
           {
              owner.ToJson();
              owner.Property(x => x.XP);
           });

         entity.HasMany(x => x.KeyAttributes)
               .WithMany()
               .UsingEntity(x => x.ToTable("ClassKeyAttributes"));
      });


      modelBuilder.Entity<CurrencyDefinition>(entity =>
      {
         entity.HasKey(x => x.Id);
         entity.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new CurrencyDefinitionId(id))
            .HasColumnType("INTEGER")
            .ValueGeneratedOnAdd()
            .UseAutoincrement();
         //.HasAnnotation("Sqlite:Autoincrement", true);
      });

      modelBuilder.Entity<Delve>(entity =>
      {
         entity.HasKey(x => x.Id);
         entity.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new DelveId(id))
            .HasColumnType("INTEGER")
            .ValueGeneratedOnAdd()
            .UseAutoincrement();
         //.HasAnnotation("Sqlite:Autoincrement", true);
      });

      modelBuilder.Entity<GeneralXPAward>(entity =>
      {
         entity.HasKey(x => x.Id);
         entity.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new GeneralXPAwardId(id))
            .HasColumnType("INTEGER")
            .ValueGeneratedOnAdd()
            .UseAutoincrement();
         //.HasAnnotation("Sqlite:Autoincrement", true);

         entity.HasOne(x => x.Session)
               .WithMany(x => x.GeneralXPAwards)
               .HasForeignKey(x => x.SessionId)
               .OnDelete(DeleteBehavior.Cascade);

         entity.HasOne(x => x.Delve)
               .WithMany(x => x.GeneralXPAwards)
               .HasForeignKey(x => x.DelveId)
               .OnDelete(DeleteBehavior.SetNull);

         entity.HasMany(d => d.Characters)
               .WithMany()
               .UsingEntity(x => x.ToTable("CharacterXPAward"));
      });

      modelBuilder.Entity<MonsterEntry>(entity =>
      {
         entity.HasKey(x => x.Id);
         entity.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new MonsterEntryId(id))
            .HasColumnType("INTEGER")
            .ValueGeneratedOnAdd()
            .UseAutoincrement();
         //.HasAnnotation("Sqlite:Autoincrement", true);

         entity.HasOne(x => x.Session)
               .WithMany(x => x.Monsters)
               .HasForeignKey(x => x.SessionId)
               .OnDelete(DeleteBehavior.Cascade);

         entity.HasOne(x => x.Delve)
               .WithMany(x => x.Monsters)
               .HasForeignKey(x => x.DelveId)
               .OnDelete(DeleteBehavior.SetNull);
      });

      //modelBuilder.Entity<Session>(entity =>
      //{
      //   entity.HasKey(x => x.Id);
      //   entity.Property(x => x.Id)
      //      .HasConversion(x => x.Value, id => new SessionId(id))
      //      .ValueGeneratedOnAdd()
      //      .HasAnnotation("Sqlite:Autoincrement", true);

      //   entity.HasMany(x => x.Characters)
      //         .WithMany()
      //         .UsingEntity(x => x.ToTable("SessionCharacters"));

      //});

      modelBuilder.Entity<Session>(entity =>
      {
         entity.HasKey(x => x.Id);
         entity.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new SessionId(id))
            .HasColumnType("INTEGER")
            .ValueGeneratedOnAdd()
            .UseAutoincrement();
         //.HasAnnotation("Sqlite:Autoincrement", true);
      });

      modelBuilder.Entity<SessionCharacter>(entity =>
      {
         entity.HasKey(x => new { x.SessionId, x.CharacterId });

         entity.Property(x => x.SessionId)
            .HasConversion(x => x.Value, value => new SessionId(value));

         entity.Property(x => x.CharacterId)
            .HasConversion(x => x.Value, value => new CharacterId(value));

         entity
            .HasOne(x => x.Character)
            .WithMany()
            .HasForeignKey(x => x.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);

         entity
            .HasOne(x => x.Session)
            .WithMany(x => x.Characters)
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
      });

      modelBuilder.Entity<DelveCharacter>(entity =>
      {
         entity.HasKey(x => new { x.DelveId, x.CharacterId });

         entity.Property(x => x.DelveId)
            .HasConversion(x => x.Value, value => new DelveId(value));

         entity.Property(x => x.CharacterId)
            .HasConversion(x => x.Value, value => new CharacterId(value));

         entity
            .HasOne(x => x.Character)
            .WithMany()
            .HasForeignKey(x => x.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);

         entity
            .HasOne<Delve>(x => x.Delve)
            .WithMany(x => x.Characters)
            .HasForeignKey(x => x.DelveId)
            .OnDelete(DeleteBehavior.Cascade);
      });

      modelBuilder.Entity<SessionDelve>(entity =>
      {
         entity.HasKey(x => x.Id);
         entity.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new SessionDelveId(id))
            .HasColumnType("INTEGER")
            .ValueGeneratedOnAdd()
            .UseAutoincrement();
         //.HasAnnotation("Sqlite:Autoincrement", true);

         entity.HasIndex(x => new { x.SessionId, x.DelveId })
               .IsUnique();

         entity.HasOne(x => x.Session)
               .WithMany(x => x.Delves)
               .HasForeignKey(x => x.SessionId)
               .OnDelete(DeleteBehavior.Cascade);

         entity.HasOne(x => x.Delve)
               .WithMany(x => x.Sessions)
               .HasForeignKey(x => x.DelveId)
               .OnDelete(DeleteBehavior.Cascade);
      });

      modelBuilder.Entity<SessionTrack>(entity =>
      {
         entity.HasKey(x => x.Id);
         entity.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new SessionTrackId(id))
            .HasColumnType("INTEGER")
            .ValueGeneratedOnAdd()
            .UseAutoincrement();
         //.HasAnnotation("Sqlite:Autoincrement", true);

         entity
            .HasMany(x => x.Sessions)
            .WithOne(z => z.SessionTrack)
            .HasForeignKey(z => z.SessionTrackId)
            .OnDelete(DeleteBehavior.Cascade);

         entity
            .HasMany(x => x.Delves)
            .WithOne(z => z.SessionTrack)
            .HasForeignKey(z => z.SessionTrackId)
            .OnDelete(DeleteBehavior.Cascade);

         entity.HasMany<Character>()
            .WithMany()
            .UsingEntity<SessionTrackCharacter>(
               join => join.HasOne(j => j.Character).WithMany(),
               join => join.HasOne(j => j.SessionTrack).WithMany(x => x.Characters));
      });

      modelBuilder.Entity<SessionTrackCharacter>(entity =>
      {
         entity.HasKey(x => new { x.SessionTrackId, x.CharacterId });
         entity.Property(x => x.SessionTrackId).HasConversion(x => x.Value, id => new SessionTrackId(id));
         entity.Property(x => x.CharacterId).HasConversion(x => x.Value, id => new CharacterId(id));

         entity.Navigation(x => x.SessionTrack).IsRequired();
         entity.Navigation(x => x.Character).IsRequired();
      });

      modelBuilder.Entity<TreasureEntry>(entity =>
      {
         entity.HasKey(x => x.Id);
         entity.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new TreasureEntryId(id))
            .HasColumnType("INTEGER")
            .ValueGeneratedOnAdd()
            .UseAutoincrement();
         //.HasAnnotation("Sqlite:Autoincrement", true);

         entity.HasOne(x => x.Session)
               .WithMany(x => x.Treasures)
               .HasForeignKey(x => x.SessionId)
               .OnDelete(DeleteBehavior.Cascade);

         entity.HasOne(x => x.Delve)
               .WithMany(x => x.Treasures)
               .HasForeignKey(x => x.DelveId)
               .OnDelete(DeleteBehavior.SetNull);

         entity.ComplexProperty(x => x.Location, b =>
         {
            b.Property(r => r.Type).HasColumnName("LocType").IsRequired();
            b.Property(r => r.CharacterId).HasColumnName("LocCharacterId")
             .HasConversion<int?>(x => x.HasValue ? x.Value.Value : null, id => id.HasValue ? new CharacterId(id.Value) : null);
            b.Property(r => r.StoreDescription).HasColumnName("LocStore");

            b.IsRequired();
         });

         entity.ComplexProperty(x => x.MagicItemDetails, b =>
         {
            b.Property(x => x.ApparentValue);
            b.Property(x => x.IdentificationStatus);
         });
      });

   }
}
