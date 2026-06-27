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

   static readonly CampaignId Sole = new(1);

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
         entity.Property(x => x.Id).HasConversion(x => x.Id, id => new AttributeDefinitionId(id));
      });

      modelBuilder.Entity<CampaignSettings>(entity =>
      {
         entity.HasKey(x => x.Id);
         entity.Property(x => x.Id).HasConversion(x => x.Id, id => new CampaignId(id));
      });

      modelBuilder.Entity<Character>(entity =>
      {
         entity.HasKey(x => x.Id);
         entity.Property(x => x.Id).HasConversion(x => x.Id, id => new CharacterId(id));

         entity.HasOne(x => x.Class)
               .WithMany()
               .HasForeignKey(a => a.ClassId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);
      });

      modelBuilder.Entity<ClassDefinition>(entity =>
      {
         entity.HasKey(x => x.Id);
         entity.Property(x => x.Id).HasConversion(x => x.Id, id => new ClassDefinitionId(id));

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
         entity.Property(x => x.Id).HasConversion(x => x.Id, id => new CurrencyDefinitionId(id));
      });

      modelBuilder.Entity<Delve>(entity =>
      {
         entity.HasKey(x => x.Id);
         entity.Property(x => x.Id).HasConversion(x => x.Id, id => new DelveId(id));
      });

      modelBuilder.Entity<GeneralXPAward>(entity =>
      {
         entity.HasKey(x => x.Id);
         entity.Property(x => x.Id).HasConversion(x => x.Id, id => new GeneralXPAwardId(id));

         entity.HasOne(x => x.SessionDelve)
               .WithMany(x => x.GeneralXPAwards)
               .HasForeignKey(x => x.SessionDelveId)
               .OnDelete(DeleteBehavior.Cascade);

         entity.HasMany(d => d.Characters)
               .WithMany()
               .UsingEntity(x => x.ToTable("CharacterXPAward"));
      });

      modelBuilder.Entity<MonsterEntry>(entity =>
      {
         entity.HasKey(x => x.Id);
         entity.Property(x => x.Id).HasConversion(x => x.Id, id => new MonsterEntryId(id));

         entity.HasOne(x => x.SessionDelve)
               .WithMany(x => x.Monsters)
               .HasForeignKey(x => x.SessionDelveId)
               .OnDelete(DeleteBehavior.Cascade);
      });

      modelBuilder.Entity<Session>(entity =>
      {
         entity.HasKey(x => x.Id);
         entity.Property(x => x.Id).HasConversion(x => x.Id, id => new SessionId(id));

         entity.HasMany(x => x.Characters)
               .WithMany()
               .UsingEntity(x => x.ToTable("SessionCharacters"));

      });

      modelBuilder.Entity<SessionDelve>(entity =>
      {
         entity.HasKey(x => x.Id);
         entity.Property(x => x.Id).HasConversion(x => x.Id, id => new SessionDelveId(id));

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
         entity.Property(x => x.Id).HasConversion(x => x.Id, id => new SessionTrackId(id));

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
         entity.Property(x => x.SessionTrackId).HasConversion(x => x.Id, id => new SessionTrackId(id));
         entity.Property(x => x.CharacterId).HasConversion(x => x.Id, id => new CharacterId(id));

         entity.Navigation(x => x.SessionTrack).IsRequired();
         entity.Navigation(x => x.Character).IsRequired();
      });

      modelBuilder.Entity<TreasureEntry>(entity =>
      {
         entity.HasKey(x => x.Id);
         entity.Property(x => x.Id).HasConversion(x => x.Id, id => new TreasureEntryId(id));

         entity.HasOne(x => x.SessionDelve)
               .WithMany(x => x.Treasures)
               .HasForeignKey(x => x.SessionDelveId)
               .OnDelete(DeleteBehavior.Cascade);

         entity.ComplexProperty(x => x.Location, b =>
         {
           b.Property(r => r.Type).HasColumnName("LocType").IsRequired();
           b.Property(r => r.CharacterId).HasColumnName("LocCharacterId");
           b.Property(r => r.StoreDescription).HasColumnName("LocStore");

           b.IsRequired();
        });

         entity.ComplexProperty(x => x.MagicItemDetails, b =>
         {
           b.Property(x => x.TrueValue);
           b.Property(x => x.IdentificationStatus);
        });
      });

   }
}
