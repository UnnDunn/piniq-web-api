using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pinball.Api.Data;
using Pinball.Api.Services.Entities.Exceptions;
using Pinball.Entities.Api.Responses.PinballCatalog;
using Pinball.Entities.Core.Exceptions;
using Pinball.Entities.Data.Opdb;
using Pinball.Entities.Data.PinballMachines;
using Pinball.Entities.Opdb;
using Pinball.Entities.Opdb.Helpers;
using Pinball.OpdbClient.Interfaces;
using CatalogSnapshotPublishResult = Pinball.Api.Services.Entities.CatalogSnapshotPublishResult;

namespace Pinball.Api.Services.Interfaces.Impl;

public partial class PinballMachineCatalogService : IPinballMachineCatalogService
{
    private readonly PinballDbContext _dbContext;
    private readonly ILogger<PinballMachineCatalogService> _logger;
    private readonly IOpdbClient _opdbClient;

    public PinballMachineCatalogService(IOpdbClient opdbClient,
        PinballDbContext dbContext,
        ILogger<PinballMachineCatalogService> logger)
    {
        _opdbClient = opdbClient;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task DeleteCatalogSnapshotAsync(int id)
    {
        LogActionDeletingCatalogById(id);

        // get list of published catalogs, in descending order of publish date
        var currentPublishedCatalogId = await _dbContext.CatalogSnapshots
            .Where(s => s.Published != null)
            .OrderByDescending(s => s.Published)
            .AsNoTracking()
            .Select(c => c.Id)
            .FirstOrDefaultAsync();

        // if the provided ID is first on the list, it maps to the currently-published snapshot,
        // which cannot be deleted
        if (currentPublishedCatalogId == id)
            throw new CatalogSnapshotException(id, "The current published snapshot cannot be deleted.");

        var snapshotToDelete = await _dbContext.CatalogSnapshots.FindAsync(id);

        if (snapshotToDelete == null) throw new CatalogSnapshotNotFoundException(id);

        _dbContext.CatalogSnapshots.Remove(snapshotToDelete);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Refreshes the cached metadata for all catalog snapshots in the database
    /// </summary>
    public async Task RefreshCatalogSnapshotsAsync()
    {
        LogActionRefreshingCatalogSnapshots();
        var snapshots = await _dbContext.CatalogSnapshots.ToListAsync();
        foreach (var snapshot in snapshots)
        {
            _dbContext.Entry(snapshot).State = EntityState.Modified;
            _dbContext.Entry(snapshot).Property(s => s.MachineJsonResponse).IsModified = true;
            _dbContext.Entry(snapshot).Property(s => s.MachineGroupJsonResponse).IsModified = true;
        }

        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Refreshes the cached metadata for the selected catalog snapshot in the database
    /// </summary>
    /// <param name="id">ID of the catalog snapshot to refresh</param>
    /// <exception cref="KeyNotFoundException">Thrown if a snapshot with the given ID is not found</exception>
    public async Task RefreshCatalogSnapshotAsync(int id)
    {
        var snapshotToUpdate = await _dbContext.CatalogSnapshots.FindAsync(id);

        if (snapshotToUpdate == null) throw new KeyNotFoundException($"The snapshot with id {id} was not found");

        LogActionRefreshingCatalogSnapshotForId(snapshotToUpdate.Id);
        _dbContext.Entry(snapshotToUpdate).State = EntityState.Modified;
        _dbContext.Entry(snapshotToUpdate).Property(s => s.MachineJsonResponse).IsModified = true;
        _dbContext.Entry(snapshotToUpdate).Property(s => s.MachineGroupJsonResponse).IsModified = true;
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Retrieves a single catalog snapshot including machine data
    /// </summary>
    /// <param name="id">ID of the snapshot to retrieve</param>
    /// <returns>The selected catalog snapshot, or null if one matching the indicated ID was not found</returns>
    public async Task<CatalogSnapshot?> GetCatalogSnapshotAsync(int id)
    {
        LogActionGettingCatalogSnapshotById(id);
        var result = await _dbContext
            .CatalogSnapshots
            .AsNoTracking()
            .Select(o => new CatalogSnapshot
            {
                Id = o.Id,
                Imported = o.Imported,
                Published = o.Published,
                Machines = o.Machines,
                MachineGroups = o.MachineGroups,
                MachineCount = o.MachineCount,
                MachineGroupCount = o.MachineGroupCount,
                ManufacturerCount = o.ManufacturerCount,
                KeywordCount = o.KeywordCount,
                NewestMachine = o.NewestMachine,
                Created = o.Created,
                Updated = o.Updated
            })
            .FirstOrDefaultAsync(o => o.Id == id);
        return result;
    }

    /// <summary>
    /// Retrieves a collection of catalog snapshots matching the given IDs
    /// </summary>
    /// <param name="ids">IDs of the snapshots to retrieve</param>
    /// <returns>A list of the selected snapshots. The list will be empty if matching snapshots were not found</returns>
    public async Task<List<CatalogSnapshot>> GetCatalogSnapshotsAsync(IEnumerable<int> ids)
    {
        using var log = _logger.BeginScope("Querying database for catalog snapshots with IDs {ids}", ids);
        var result = await _dbContext
            .CatalogSnapshots
            .AsNoTracking()
            .Select(o => new CatalogSnapshot
            {
                Id = o.Id,
                Imported = o.Imported,
                Published = o.Published,
                Machines = o.Machines,
                MachineGroups = o.MachineGroups,
                MachineCount = o.MachineCount,
                MachineGroupCount = o.MachineGroupCount,
                ManufacturerCount = o.ManufacturerCount,
                KeywordCount = o.KeywordCount,
                NewestMachine = o.NewestMachine,
                Created = o.Created,
                Updated = o.Updated
            })
            .Where(s => ids.Contains(s.Id))
            .ToListAsync();
        _logger.LogInformation("Found {count} catalog snapshots matching criteria", result.Count);
        return result;
    }

    /// <summary>
    /// Gets the current published snapshot
    /// </summary>
    /// <returns>The current published snapshot, if it exists, or null if it does not</returns>
    public async Task<CatalogSnapshot?> GetPublishedCatalogSnapshotAsync()
    {
        LogGettingPublishedCatalogSnapshots();
        var result = await _dbContext.CatalogSnapshots
            .Select(o => new CatalogSnapshot
            {
                Id = o.Id,
                Imported = o.Imported,
                Published = o.Published,
                Machines = o.Machines,
                MachineGroups = o.MachineGroups,
                MachineCount = o.MachineCount,
                MachineGroupCount = o.MachineGroupCount,
                ManufacturerCount = o.ManufacturerCount,
                KeywordCount = o.KeywordCount,
                NewestMachine = o.NewestMachine,
                Created = o.Created,
                Updated = o.Updated
            })
            .Where(s => s.Published != null)
            .OrderByDescending(s => s.Published)
            .AsNoTracking()
            .FirstOrDefaultAsync();
        return result;
    }

    public async Task<CatalogSnapshot> ImportNewCatalogSnapshotAsync()
    {
        LogActionImportingCatalogSnapshots();
        // check database to see when the last snapshot was retrieved
        var latestSnapshot = await _dbContext.CatalogSnapshots.OrderByDescending(s => s.Imported).FirstOrDefaultAsync();

        if (latestSnapshot != null)
            if (DateTimeOffset.Now - latestSnapshot.Imported < TimeSpan.FromHours(1))
                throw new OpdbException(
                    $"The newest snapshot cannot be retrieved right now. Please wait until {latestSnapshot.Imported.AddHours(1)}.");

        // get the latest snapshot from Opdb
        var exportMachinesResponse = await _opdbClient.ExportAsync();
        var exportMachineGroupsResponse = await _opdbClient.ExportGroupsAsync();

        // save the snapshot to the database
        var catalogExportEntry = new OpdbCatalogSnapshot
        {
            Imported = DateTimeOffset.Now,
            MachineJsonResponse = exportMachinesResponse.JsonResponse,
            MachineGroupJsonResponse = exportMachineGroupsResponse.JsonResponse
        };

        _dbContext.CatalogSnapshots.Add(catalogExportEntry);
        await _dbContext.SaveChangesAsync();

        return catalogExportEntry;
    }

    public async Task<CatalogSnapshotPublishResult> PublishCatalogSnapshotAsync(int? id = null)
    {
        LogActionPublishCatalogSnapshot();
        // get all snapshots
        var allSnapshots = await _dbContext.CatalogSnapshots.OrderByDescending(s => s.Imported).ToListAsync();
        
        // get selected or latest unpublished snapshot
        var snapshotToPublish = id is not null 
        ? await _dbContext.CatalogSnapshots.FindAsync(id)
        : await _dbContext.CatalogSnapshots.OrderByDescending(s => s.Imported).FirstOrDefaultAsync();

        if (snapshotToPublish == null)
        {
            if (id is not null)
            {
                LogIssueSelectedCatalogSnapshotNotFound(id.Value);
                throw new CatalogSnapshotNotFoundException(id);
            }
            LogIssueNoCatalogSnapshotsFound();
            throw new CatalogSnapshotException("No snapshots found");
        }
        
        LogProcessLatestCatalogSnapshotIdentified(snapshotToPublish.Id, snapshotToPublish.Imported);
        if (snapshotToPublish.Published is not null)
        {
            LogIssueCatalogSnapshotAlreadyPublished(snapshotToPublish.Id, snapshotToPublish.Published.Value);
            throw new CatalogSnapshotException(snapshotToPublish.Id, "The selected snapshot has already been published");
        }
        
        // check to see if there are any newer published snapshots
        var newerSnapshots = allSnapshots.Any(s => s.Published is not null && s.Imported > snapshotToPublish.Imported);
        if (newerSnapshots)
        {
            throw new CatalogSnapshotException("There are newer published snapshots");
        }

        var machines = snapshotToPublish.Machines;
        var machineGroups = snapshotToPublish.MachineGroups;

        if (machines is null || machineGroups is null)
            throw new Exception("Machine snapshot JSON could not be parsed.");
        await LoadMachineGroupsAsync(machineGroups);
        
        // hold a dictionary of Machine Group Ids for later mapping
        var machineGroupIds = await _dbContext.PinballMachineGroups.ToDictionaryAsync(p => p.OpdbId, p => p.Id);

        // manufacturers
        var manufacturers = machines
            .Where(m => m.Manufacturer != null)
            .Select(m => m.Manufacturer).Distinct(new ManufacturerComparer()).ToList();
        await LoadManufacturersAsync(manufacturers);

        // machines
        await LoadPinballMachinesAsync(machines);

        // hold a dictionary of machine Ids for later mapping
        var machineIds = await _dbContext.PinballMachines.ToDictionaryAsync(p => p.OpdbId, q => q.Id);
        
        // keywords
        await LoadKeywordsAsync(machines);

        // keyword mappings
        await MapPinballKeywordsAsync(machines, machineIds);

        // mark all snapshots as unpublished
        foreach (var snapshot in allSnapshots)
        {
            snapshot.Published = null;
        }
        
        // mark snapshot as published
        snapshotToPublish.Published = DateTimeOffset.Now;
        LogProcessMarkingCatalogSnapshotAsPublished(snapshotToPublish.Id, snapshotToPublish.Published.Value);
        await _dbContext.SaveChangesAsync();

        // // get results
        var machineTotal = await _dbContext.PinballMachines.CountAsync();
        var machineGroupsTotal = await _dbContext.PinballMachineGroups.CountAsync();
        var manufacturersTotal = await _dbContext.PinballManufacturers.CountAsync();
        var keywordsTotal = await _dbContext.PinballKeywords.CountAsync();
        LogDebugCatalogSnapshotPublishCounts(snapshotToPublish.Id, machineTotal, machineGroupsTotal, manufacturersTotal,
            keywordsTotal);
        
        var result = new CatalogSnapshotPublishResult
        {
            MachineTotal = machineTotal,
            MachineGroupTotal = machineGroupsTotal,
            ManufacturerTotal = manufacturersTotal,
            KeywordTotal = keywordsTotal,
            SnapshotId = snapshotToPublish.Id,
            Imported = snapshotToPublish.Imported
        };
        
        return result;
    }

    public async Task<IEnumerable<CatalogSnapshot>> GetAllCatalogSnapshotsAsync()
    {
        var result = await _dbContext
            .CatalogSnapshots
            .AsNoTracking()
            .Select(o => new CatalogSnapshot
            {
                Id = o.Id,
                Imported = o.Imported,
                Published = o.Published,
                MachineCount = o.MachineCount,
                MachineGroupCount = o.MachineGroupCount,
                ManufacturerCount = o.ManufacturerCount,
                KeywordCount = o.KeywordCount,
                NewestMachine = o.NewestMachine,
                Created = o.Created,
                Updated = o.Updated
            })
            .ToListAsync();
        return result;
    }

    public async Task ResetCatalogAsync()
    {
        LogActionResettingCatalog();
        // unpublish catalogs
        var catalogSnapshots = await _dbContext.CatalogSnapshots.ToListAsync();

        foreach (var snapshot in catalogSnapshots)
        {
            LogProgressMarkingSnapshotUnpublished(snapshot.Id);
            snapshot.Published = null;
        }

        await _dbContext.SaveChangesAsync();

        // delete keywords
        await _dbContext.PinballKeywords.ExecuteDeleteAsync();
        
        // delete features
        await _dbContext.PinballFeatures.ExecuteDeleteAsync();
        
        // delete pinball machines
        await _dbContext.PinballMachines.ExecuteDeleteAsync();
        
        // delete manufacturers
        await _dbContext.PinballManufacturers.ExecuteDeleteAsync();
        
        await _dbContext.SaveChangesAsync();
    }

    public async Task<int> UpdateChangelogsAsync()
    {
        // get most recent changelog date
        var latestChangelog = await _dbContext.OpdbChangelogs.OrderByDescending(c => c.Date).FirstOrDefaultAsync();

        var response = await _opdbClient.GetChangelogAsync(latestChangelog?.Date);

        var jsonOptions = new JsonSerializerOptions();
        jsonOptions.Converters.Add(new NullableDateTimeConverterWithParseFallback());
        jsonOptions.Converters.Add(new ChangelogActionConverter());
        var newChangelogs = JsonSerializer.Deserialize<IEnumerable<Changelog>>(response.JsonResponse, jsonOptions);

        if (newChangelogs is null) return 0;

        // get all existing changelogs 
        var savedChangelogs = await _dbContext.OpdbChangelogs.ToDictionaryAsync(cl => cl.Id);

        foreach (var newChangelog in newChangelogs)
        {
            // get existing changelog

            savedChangelogs.TryGetValue(newChangelog.ChangelogId, out var savedChangelog);
            if (savedChangelog != null)
            {
                savedChangelog.OpdbId = newChangelog.OpdbIdDeleted?.ToString() ?? string.Empty;
                savedChangelog.Action = (OpdbChangelogAction)(int)newChangelog.Action;
                savedChangelog.NewOpdbId = newChangelog.OpdbIdReplacement?.ToString();
                savedChangelog.Date = newChangelog.CreatedAt;
            }
            else
            {
                var createdChangelog = new OpdbChangelog
                {
                    Id = newChangelog.ChangelogId,
                    OpdbId = newChangelog.OpdbIdDeleted?.ToString() ?? string.Empty,
                    Action = (OpdbChangelogAction)(int)newChangelog.Action,
                    NewOpdbId = newChangelog.OpdbIdReplacement?.ToString(),
                    Date = newChangelog.CreatedAt
                };
                _dbContext.OpdbChangelogs.Add(createdChangelog);
            }
        }

        await _dbContext.SaveChangesAsync();

        return await _dbContext.OpdbChangelogs.CountAsync();
    }

    public async Task<IEnumerable<OpdbChangelog>> GetChangelogsAsync()
    {
        var result = await _dbContext.OpdbChangelogs.AsNoTracking().ToListAsync();
        return result;
    }

    public async Task<Dictionary<string, int>> GetAllMachineTypesAsync()
    {
        var latestSnapshot = await _dbContext.CatalogSnapshots.AsNoTracking().OrderByDescending(s => s.Created)
            .FirstOrDefaultAsync();

        if (latestSnapshot == null) throw new InvalidOperationException("No snapshots exist to get machine types for");

        var machines = latestSnapshot.Machines;

        if (machines is null) throw new Exception("Machine snapshot JSON could not be parsed");
        var machineTypeGroups = machines.GroupBy(m => m.MachineType)
            .ToDictionary(g => g.Key?.ToString() ?? "Unknown", g => g.Count());

        return machineTypeGroups;
    }

    public async Task<Dictionary<string, int>> GetAllDisplayTypesAsync()
    {
        var latestSnapshot = await _dbContext.CatalogSnapshots.AsNoTracking().OrderByDescending(s => s.Created)
            .FirstOrDefaultAsync();

        if (latestSnapshot == null) throw new InvalidOperationException("No snapshots exist to get display types for");

        var machines = latestSnapshot.Machines;

        if (machines is null) throw new Exception("Machine snapshot JSON could not be parsed");
        var displayTypeGroups = machines.GroupBy(m => m.Display ?? "null")
            .ToDictionary(g => g.Key.ToString(), g => g.Count());

        return displayTypeGroups;
    }

    private async Task LoadMachineGroupsAsync(IEnumerable<MachineGroup> machineGroups)
    {
        // convert collection to db entities
        var dbMachineGroups = machineGroups
            .Distinct(new MachineGroupComparer())
            .Select(m => new PinballMachineGroup
            {
                OpdbId = m.OpdbId.ToString(),
                Name = m.Name,
                ShortName = m.Shortname,
                Description = m.Description
            }).ToList();

        LogProcessLoadingMachineGroupsToCatalog(dbMachineGroups.Count);
        await _dbContext.BulkInsertOrUpdateAsync(dbMachineGroups);
    }

    private async Task LoadManufacturersAsync(IEnumerable<Manufacturer?> manufacturers)
    {
        LogProcessLoadingManufacturersToActiveCatalog();
        // get existing manufacturers
        var savedManufacturers = await _dbContext.PinballManufacturers.ToDictionaryAsync(m => m.Id);
        var newManufacturers = manufacturers.Select(m =>
        {
            Debug.Assert(m != null, nameof(m) + " != null");
            return new PinballMachineManufacturer
            {
                Id = m.ManufacturerId,
                Name = m.Name,
                FullName = m.FullName
            };
        });

        foreach (var newManufacturer in newManufacturers)
            if (savedManufacturers.TryGetValue(newManufacturer.Id, out var savedManufacturer))
                _dbContext.Entry(savedManufacturer).CurrentValues.SetValues(newManufacturer);
            else
                _dbContext.Add(newManufacturer);
        
        await _dbContext.SaveChangesAsync();
    }

    private async Task LoadPinballMachinesAsync(IEnumerable<Machine> machines)
    {
        var newMachines = machines.Select(m => new PinballMachine
        {
            OpdbId = m.OpdbId.ToString(),
            CommonName = m.CommonName,
            Name = m.Name,
            IpdbId = m.IpdbId,
            ManufactureDate = m.ManufactureDate,
            ManufacturerId = m.Manufacturer?.ManufacturerId,
            PlayerCount = (short)m.PlayerCount,
            TypeId = m.MachineType
        }).ToList();

        await _dbContext.BulkInsertOrUpdateAsync(newMachines);
    }

    private async Task LoadKeywordsAsync(IEnumerable<Machine> machines)
    {
        var newKeywords = machines.SelectMany(m => m.Keywords ?? []).Distinct().ToList();

        var savedKeywords = await _dbContext.PinballKeywords.ToDictionaryAsync(k => k.Name, k => k.Id);

        var newKeywordEntities = newKeywords
            .Where(k => !savedKeywords.ContainsKey(k))
            .Select(kw => new PinballMachineKeyword
            {
                Id = Guid.NewGuid(),
                Name = kw
            }).ToList();

        await _dbContext.BulkInsertAsync(newKeywordEntities);
    }

    private async Task MapPinballKeywordsAsync(IEnumerable<Machine> machines, Dictionary<string, int> machineIds)
    {
        var savedKeywords = await _dbContext.PinballKeywords.ToDictionaryAsync(k => k.Name, k => k.Id);

        var keywordMappings = new List<PinballMachineKeywordMapping>();

        foreach (var machine in machines)
        {
            var mappings = machine.Keywords?.Select(kw => new PinballMachineKeywordMapping
            {
                KeywordId = savedKeywords[kw],
                MachineId = machineIds[machine.OpdbId]
            });
            if (mappings is not null)
                keywordMappings.AddRange(mappings);
        }

        await _dbContext.BulkInsertOrUpdateAsync(keywordMappings);
    }

    #region Logging

    // Methods for writing source-generated logging statements
    // All logging statements in this service must have event IDs "10xx"

    [LoggerMessage(EventId = 1001, Level = LogLevel.Information, Message = "Deleting Catalog Snapshot with ID {id}.")]
    private partial void LogActionDeletingCatalogById(int id);

    [LoggerMessage(EventId = 1002, Level = LogLevel.Information, Message = "Getting Catalog Snapshot with ID {id}.")]
    private partial void LogActionGettingCatalogSnapshotById(int id);

    [LoggerMessage(EventId = 1003, Level = LogLevel.Information, Message = "Getting all published Catalog Snapshots.")]
    private partial void LogGettingPublishedCatalogSnapshots();

    [LoggerMessage(EventId = 1004, Level = LogLevel.Information, Message = "Importing new Catalog Snapshot.")]
    private partial void LogActionImportingCatalogSnapshots();

    [LoggerMessage(EventId = 1005, Level = LogLevel.Information, Message = "Publishing latest Catalog Snapshot.")]
    private partial void LogActionPublishCatalogSnapshot();

    [LoggerMessage(EventId = 1006, Level = LogLevel.Information,
        Message = "Could not publish latest Catalog Snapshot because no Catalog Snapshots were found.")]
    private partial void LogIssueNoCatalogSnapshotsFound();

    [LoggerMessage(EventId = 1007, Level = LogLevel.Information,
        Message = "Latest Catalog Snapshot found with id {id}, imported on {importedDate}.")]
    private partial void LogProcessLatestCatalogSnapshotIdentified(int id, DateTimeOffset importedDate);

    [LoggerMessage(EventId = 1008, Level = LogLevel.Information,
        Message =
            "Could not publish latest Catalog Snapshot with id {id} because it was already published on {publishedDate}")]
    private partial void LogIssueCatalogSnapshotAlreadyPublished(int id, DateTimeOffset publishedDate);

    [LoggerMessage(EventId = 1009, Level = LogLevel.Information,
        Message = "Marking Catalog Snapshot {id} as Published on {publishedDate}.")]
    private partial void LogProcessMarkingCatalogSnapshotAsPublished(int id, DateTimeOffset publishedDate);

    [LoggerMessage(EventId = 1010,
        Level = LogLevel.Debug,
        Message =
            "Published Catalog Snapshot {id} with {machineTotal} machines, {machineGroupsTotal} groups, {manufacturersTotal} manufacturers and {keywordsTotal} keywords.")]
    private partial void LogDebugCatalogSnapshotPublishCounts(int id, int machineTotal, int machineGroupsTotal,
        int manufacturersTotal, int keywordsTotal);

    [LoggerMessage(EventId = 1011, Level = LogLevel.Information,
        Message = "Loading {machineGroupCount} machine groups to active catalog.")]
    private partial void LogProcessLoadingMachineGroupsToCatalog(int machineGroupCount);

    [LoggerMessage(EventId = 1012, Level = LogLevel.Information, Message = "Loading manufacturers to active Catalog.")]
    private partial void LogProcessLoadingManufacturersToActiveCatalog();

    [LoggerMessage(EventId = 1013, Level = LogLevel.Information, Message = "Resetting active catalog.")]
    private partial void LogActionResettingCatalog();

    [LoggerMessage(EventId = 1014, Level = LogLevel.Debug, Message = "Marking snapshot {id} as unpublished.")]
    private partial void LogProgressMarkingSnapshotUnpublished(int id);

    [LoggerMessage(EventId = 1015, Level = LogLevel.Debug, Message = "Updating changelogs from OPDB.")]
    private partial void LogActionUpdatingChangelogs();

    [LoggerMessage(EventId = 1016, Level = LogLevel.Information,
        Message = "Refreshing all catalog snapshot cached data")]
    private partial void LogActionRefreshingCatalogSnapshots();

    [LoggerMessage(EventId = 1017, Level = LogLevel.Information,
        Message = "Refreshing catalog snapshot cached data for ID {catalogSnapshotId}")]
    private partial void LogActionRefreshingCatalogSnapshotForId(int catalogSnapshotId);

    [LoggerMessage(EventId = 1018, Level = LogLevel.Error,
        Message = "Could not publish catalog snapshot with id {id} because it was not found.")]
    private partial void LogIssueSelectedCatalogSnapshotNotFound(int id);

    #endregion
}