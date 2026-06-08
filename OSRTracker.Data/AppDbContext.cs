using Microsoft.EntityFrameworkCore;
using OSRTracker.Core.Models;

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

    private static readonly Func<AppDbContext, Task<CampaignSettings?>> _getCampaign =
        EF.CompileAsyncQuery((AppDbContext ctx) => 
        ctx.CampaignSettings.AsNoTracking().FirstOrDefault(x => x.Id == 1));

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
    public DbSet<TreasureEntry> TreasureEntries { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AttributeDefinition>(entity =>
        {
            entity.HasKey(x => x.Id);
        });

        modelBuilder.Entity<CampaignSettings>(entity =>
        {
            entity.HasKey(x => x.Id);
        });

        modelBuilder.Entity<AttributeDefinition>(entity =>
        {
            entity.HasKey(x => x.Id);
        });

        modelBuilder.Entity<Character>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.Class)
                .WithMany()
                .HasForeignKey(a => a.ClassId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ClassDefinition>(entity =>
        {
            entity.HasKey(x => x.Id);

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
        });

        modelBuilder.Entity<Delve>(entity =>
        {
            entity.HasKey(x => x.Id);
        });

        modelBuilder.Entity<GeneralXPAward>(entity =>
        {
            entity.HasKey(x => x.Id);

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

            entity.HasOne(x => x.SessionDelve)
                .WithMany(x => x.Monsters)
                .HasForeignKey(x => x.SessionDelveId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasMany(x => x.Characters)
                .WithMany()
                .UsingEntity(x => x.ToTable("SessionCharacters"));

        });

        modelBuilder.Entity<SessionDelve>(entity =>
        {
            entity.HasKey(x => x.Id);

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

        modelBuilder.Entity<TreasureEntry>(entity =>
        {
            entity.HasKey(x => x.Id);

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
