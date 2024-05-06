using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using Pinball.Entities.Data.Helpers;
using Pinball.Entities.Data.Opdb;
using Pinball.Entities.Data.PinballMachines;
using Pinball.Entities.Opdb.Helpers;

namespace Pinball.Api.Data;

public partial class PinballDbContext : DbContext
{
    private const string SqlCaseInsensitiveCollationName = "SQL_Latin1_General_CP1_CS_AS";

    private readonly string _caseInsensitiveCollationName = SqlCaseInsensitiveCollationName;
    private readonly ILogger<PinballDbContext> _logger;

    public PinballDbContext(DbContextOptions<PinballDbContext> options, ILogger<PinballDbContext> logger) :
        base(options)
    {
        _logger = logger;
        SavingChanges += SetCatalogSnapshotDigestInformation;
    }

    public DbSet<OpdbCatalogSnapshot> CatalogSnapshots { get; set; } = null!;
    public DbSet<OpdbChangelog> OpdbChangelogs { get; set; } = null!;
    public DbSet<PinballMachine> PinballMachines { get; set; } = null!;
    public DbSet<PinballMachineFeature> PinballFeatures { get; set; } = null!;
    public DbSet<PinballMachineType> PinballTypes { get; set; } = null!;
    public DbSet<PinballMachineManufacturer> PinballManufacturers { get; set; } = null!;
    public DbSet<PinballMachineKeyword> PinballKeywords { get; set; } = null!;
    public DbSet<PinballMachineGroup> PinballMachineGroups { get; set; } = null!;

    private void SetCatalogSnapshotDigestInformation(object? sender, SavingChangesEventArgs e)
    {
        LogSettingCatalogSnapshotDigestInformation();
        foreach (var entity in ChangeTracker.Entries<OpdbCatalogSnapshot>().ToList())
        {
            if (entity.State is not (EntityState.Added or EntityState.Modified)) 
                // entity unchanged, so skip
                continue;
            if (string.IsNullOrEmpty(entity.Entity.MachineJsonResponse) ||
                string.IsNullOrEmpty(entity.Entity.MachineGroupJsonResponse)) 
                // no json response content to generate digest data from, so skip
                continue;
            if (entity.State is EntityState.Modified && !entity.Property(e => e.MachineJsonResponse).IsModified &&
                !entity.Property(e => e.MachineGroupJsonResponse).IsModified)
                // json response content is unchanged, so skip
                continue;
            try
            {
                using var ms = new MemoryStream(Encoding.UTF8.GetBytes(entity.Entity.MachineJsonResponse));
                using var mgs = new MemoryStream(Encoding.UTF8.GetBytes(entity.Entity.MachineGroupJsonResponse));
                var machines =
                    JsonSerializer.Deserialize(ms, OpdbJsonSerializerContext.Default.ListMachine);
                var machineGroups = JsonSerializer.Deserialize(mgs,
                    OpdbJsonSerializerContext.Default.ListMachineGroup);

                if (machines is null || machineGroups is null)
                    continue;

                entity.Entity.Machines = machines;
                entity.Entity.MachineGroups = machineGroups;

                entity.Entity.MachineCount = machines.Count;
                entity.Entity.MachineGroupCount = machineGroups.Count;
                entity.Entity.ManufacturerCount = machines
                    .Where(m => m.Manufacturer != null)
                    .Select(m => m.Manufacturer?.ManufacturerId)
                    .Distinct()
                    .Count();
                entity.Entity.KeywordCount = machines.SelectMany(m => m.Keywords ?? [])
                    .Distinct().Count();

                var newestMachine = machines.MaxBy(m => m.ManufactureDate);
                entity.Entity.NewestMachine = newestMachine?.Name ?? "N/A";
            }
            catch (JsonException jsonException)
            {
                // log the parsing error but otherwise do nothing
                LogErrorParsingJsonResponse(jsonException, entity.Entity.Id);
            }
        }
    }

    public override void Dispose()
    {
        SavingChanges -= SetCatalogSnapshotDigestInformation;
        GC.SuppressFinalize(this);
        base.Dispose();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.AddInterceptors(new SetDateTimeValuesInterceptor());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // pinball machine
        modelBuilder.Entity<PinballMachine>().HasKey(pm => pm.Id);
        modelBuilder.Entity<PinballMachine>().HasMany(pm => pm.KeywordMappings).WithOne(km => km.Machine)
            .HasForeignKey(km => km.MachineId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<PinballMachine>().HasMany(pm => pm.FeatureMappings).WithOne(fm => fm.Machine)
            .HasForeignKey(fm => fm.MachineId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<PinballMachine>().HasOne(pm => pm.Manufacturer).WithMany(pm => pm.Machines)
            .HasForeignKey(pm => pm.ManufacturerId).OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<PinballMachine>().HasOne(pm => pm.Type).WithMany().HasForeignKey(pm => pm.TypeId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<PinballMachine>().Ignore(pm => pm.Keywords);
        modelBuilder.Entity<PinballMachine>().Ignore(pm => pm.Features);
        modelBuilder.Entity<PinballMachine>().HasOne(pm => pm.MachineGroup).WithMany(mg => mg.Machines)
            .HasForeignKey(pm => pm.MachineGroupId).OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<PinballMachine>().Property(pm => pm.Id).UseCollation(_caseInsensitiveCollationName);
        modelBuilder.Entity<PinballMachine>().Property(pm => pm.MachineGroupId)
            .UseCollation(_caseInsensitiveCollationName);

        // pinball features
        modelBuilder.Entity<PinballMachineFeatureMapping>().HasKey(pmfm => new { pmfm.FeatureId, pmfm.MachineId });
        modelBuilder.Entity<PinballMachineFeatureMapping>().Property(pmfm => pmfm.MachineId)
            .UseCollation(_caseInsensitiveCollationName);
        modelBuilder.Entity<PinballMachineFeature>().HasKey(pmf => pmf.Id);
        modelBuilder.Entity<PinballMachineFeature>().HasMany(pmf => pmf.Mappings).WithOne(pmf => pmf.Feature)
            .HasForeignKey(pmf => pmf.FeatureId).OnDelete(DeleteBehavior.Cascade);

        // pinball keywords
        modelBuilder.Entity<PinballMachineKeywordMapping>().HasKey(pmkm => new { pmkm.KeywordId, pmkm.MachineId });
        modelBuilder.Entity<PinballMachineKeywordMapping>().HasIndex(pmkm => pmkm.KeywordId);
        modelBuilder.Entity<PinballMachineKeywordMapping>().Property(pmkm => pmkm.MachineId)
            .UseCollation(_caseInsensitiveCollationName);
        modelBuilder.Entity<PinballMachineKeyword>().HasKey(pmf => pmf.Id);
        modelBuilder.Entity<PinballMachineKeyword>().HasMany(pk => pk.Mappings).WithOne(pk => pk.Keyword)
            .HasForeignKey(pk => pk.KeywordId).OnDelete(DeleteBehavior.Cascade);

        // machine types
        modelBuilder.Entity<PinballMachineType>().HasData(
            new PinballMachineType { Id = "ss", Name = "Solid-State" },
            new PinballMachineType { Id = "em", Name = "Electro-Mechanical" },
            new PinballMachineType { Id = "me", Name = "Mechanical" },
            new PinballMachineType { Id = "dmd", Name = "Dot-Matrix Display" }
        );

        // manufacturer
        modelBuilder.Entity<PinballMachineManufacturer>().Property(pmm => pmm.Id).ValueGeneratedNever();

        // machine groups
        modelBuilder.Entity<PinballMachineGroup>().Property(g => g.Id).UseCollation(_caseInsensitiveCollationName);

        modelBuilder.Entity<OpdbCatalogSnapshot>().Property(g => g.Created).HasDefaultValueSql("sysdatetimeoffset()");
        modelBuilder.Entity<OpdbCatalogSnapshot>().Property(g => g.Updated).HasDefaultValueSql("sysdatetimeoffset()");
        modelBuilder.Entity<OpdbCatalogSnapshot>().OwnsMany(cs => cs.Machines, ownedMachineBuilder =>
        {
            ownedMachineBuilder.ToJson();
            ownedMachineBuilder.Ignore(m => m.OpdbIdentifier);
            ownedMachineBuilder.OwnsOne(m => m.Manufacturer,
                ownedManufacturerBuilder => { ownedManufacturerBuilder.ToJson(); });
        });
        modelBuilder.Entity<OpdbCatalogSnapshot>().OwnsMany(cs => cs.MachineGroups, ownedMachineGroupBuilder =>
        {
            ownedMachineGroupBuilder.ToJson();
            ownedMachineGroupBuilder.Ignore(mg => mg.OpdbIdentifier);
        });
        modelBuilder.Entity<OpdbChangelog>().Property(g => g.Created).HasDefaultValueSql("sysdatetimeoffset()");
        modelBuilder.Entity<OpdbChangelog>().Property(g => g.Updated).HasDefaultValueSql("sysdatetimeoffset()");

        if (Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite") return;
        // SQLite does not have proper support for DateTimeOffset via Entity Framework Core, see the limitations
        // here: https://docs.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations
        // To work around this, when the Sqlite database provider is used, all model properties of type DateTimeOffset
        // use the DateTimeOffsetToBinaryConverter
        // Based on: https://github.com/aspnet/EntityFrameworkCore/issues/10784#issuecomment-415769754
        // This only supports millisecond precision, but should be sufficient for most use cases.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(DateTimeOffset)
                                                                           || p.PropertyType ==
                                                                           typeof(DateTimeOffset?));
            foreach (var property in properties)
                modelBuilder
                    .Entity(entityType.Name)
                    .Property(property.Name)
                    .HasConversion(new DateTimeOffsetToBinaryConverter());
        }
    }

    [LoggerMessage(EventId = 1401, Level = LogLevel.Information,
        Message = "Setting CatalogSnapshot Digest information")]
    private partial void LogSettingCatalogSnapshotDigestInformation();

    [LoggerMessage(EventId = 1402, Level = LogLevel.Error,
        Message = "Failed parsing Json Response data for snapshot Id {snapshotId}")]
    private partial void LogErrorParsingJsonResponse(JsonException ex, int snapshotId);
}