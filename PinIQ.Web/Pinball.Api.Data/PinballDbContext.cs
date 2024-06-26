using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using Pinball.Entities.Core.Entities;
using Pinball.Entities.Data.Helpers;
using Pinball.Entities.Data.Opdb;
using Pinball.Entities.Data.PinballMachines;
using Pinball.Entities.Opdb;
using Pinball.Entities.Opdb.Helpers;

namespace Pinball.Api.Data;

public partial class PinballDbContext : PinballMachineDataContext
{
    private const string SqlCaseInsensitiveCollationName = "SQL_Latin1_General_CP1_CS_AS";

    // private readonly string _caseInsensitiveCollationName = SqlCaseInsensitiveCollationName;
    private readonly ILogger<PinballDbContext> _logger;

    public PinballDbContext(DbContextOptions<PinballDbContext> options, ILogger<PinballDbContext> logger) :
        base(options)
    {
        _logger = logger;
        SavingChanges += SetCatalogSnapshotDigestInformation;
    }

    public DbSet<OpdbCatalogSnapshot> CatalogSnapshots { get; set; } = null!;
    public DbSet<CatalogChangelog> CatalogChangelogs { get; set; } = null!;
    public DbSet<OpdbChangelog> OpdbChangelogs { get; set; } = null!;


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

                foreach (var machine in machines)
                {
                    var hashCode = machine.CalculateMachineHash();
                    machine.EqualityHash = hashCode;
                }

                foreach (var machineGroup in machineGroups)
                {
                    var equalityHash = machineGroup.CalculateMachineGroupHash();
                    machineGroup.EqualityHash = equalityHash;
                }
                
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
        
        modelBuilder.Entity<CatalogChangelog>().Property(c => c.Id).ValueGeneratedNever();
        modelBuilder.Entity<CatalogChangelog>().Property(c => c.Created).HasDefaultValueSql("sysdatetimeoffset()");
        modelBuilder.Entity<CatalogChangelog>().Property(c => c.Updated).HasDefaultValueSql("sysdatetimeoffset()");
        modelBuilder.Entity<CatalogChangelog>().Property(c => c.PinballMachines)
            .HasConversion(cl => JsonSerializer.Serialize(cl!, OpdbJsonSerializerContext.Default.ChangelogEntitiesMachine),
                json => JsonSerializer.Deserialize(json, OpdbJsonSerializerContext.Default.ChangelogEntitiesMachine));
        modelBuilder.Entity<CatalogChangelog>().Property(c => c.PinballMachineGroups)
            .HasConversion(
                cl => JsonSerializer.Serialize(cl!, OpdbJsonSerializerContext.Default.ChangelogEntitiesMachineGroup),
                json => JsonSerializer.Deserialize(json,
                    OpdbJsonSerializerContext.Default.ChangelogEntitiesMachineGroup));
        modelBuilder.Entity<CatalogChangelog>().Property(c => c.PinballManufacturers)
            .HasConversion(
                cl => JsonSerializer.Serialize(cl!, OpdbJsonSerializerContext.Default.ChangelogEntitiesManufacturer),
                json => JsonSerializer.Deserialize(json,
                    OpdbJsonSerializerContext.Default.ChangelogEntitiesManufacturer));
        
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