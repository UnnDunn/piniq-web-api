using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Pinball.Api.Data;
using Pinball.Api.Data.Entities;
using Pinball.Api.Services.Entities;
using Pinball.Api.Services.Entities.Exceptions;
using Pinball.OpdbClient.Entities;
using Pinball.OpdbClient.Helpers;
using Pinball.OpdbClient.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pinball.Api.Services.Interfaces.Impl
{
    public class PinballMachineCatalogService : IPinballMachineCatalogService
    {
        private readonly IOpdbClient _opdbClient;
        private readonly PinballDbContext _dbContext;

        public PinballMachineCatalogService(IOpdbClient opdbClient, PinballDbContext dbContext)
        {
            _opdbClient = opdbClient;
            _dbContext = dbContext;
        }

        public async Task DeleteCatalogSnapshotAsync(int id)
        {
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
            {
                throw new CatalogSnapshotException(id, "The current published snapshot cannot be deleted.");
            }

            var snapshotToDelete = await _dbContext.CatalogSnapshots.FindAsync(id);

            if (snapshotToDelete == null)
            {
                throw new KeyNotFoundException($"The snapshot with id {id} was not found");
            }

            _dbContext.CatalogSnapshots.Remove(snapshotToDelete);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<OpdbCatalogSnapshot?> GetCatalogSnapshotAsync(int id)
        {
            var result = await _dbContext.CatalogSnapshots.FindAsync(id);
            return result;
        }

        public async Task<OpdbCatalogSnapshot?> GetPublishedCatalogSnapshotAsync()
        {
            var result = await _dbContext.CatalogSnapshots
                                         .Where(s => s.Published != null)
                                         .OrderByDescending(s => s.Published)
                                         .AsNoTracking()
                                         .FirstOrDefaultAsync();
            return result;
        }

        public async Task<OpdbCatalogSnapshot> ImportNewCatalogSnapshotAsync()
        {
            // check database to see when the last snapshot was retrieved
            var latestSnapshot = await _dbContext.CatalogSnapshots.OrderByDescending(s => s.Imported).FirstOrDefaultAsync();

            if(latestSnapshot != null)
            {

                if (DateTime.UtcNow - latestSnapshot.Imported < TimeSpan.FromHours(1))
                    throw new OpdbException($"The newest snapshot cannot be retrieved right now. Please wait until {latestSnapshot.Imported.AddHours(1)}.");
            }

            // get the latest snapshot from Opdb
            var exportMachinesResponse = await _opdbClient.ExportAsync();
            var exportMachineGroupsResponse = await _opdbClient.ExportGroupsAsync();

            // save the snapshot to the database
            var catalogExportEntry = new OpdbCatalogSnapshot
            {
                Imported = DateTime.UtcNow,
                MachineJsonResponse = exportMachinesResponse.JsonResponse,
                MachineGroupJsonResponse = exportMachineGroupsResponse.JsonResponse,
            };

            _dbContext.CatalogSnapshots.Add(catalogExportEntry);
            await _dbContext.SaveChangesAsync();

            return catalogExportEntry;
        }

        public async Task<CatalogSnapshotPublishResult> PublishCatalogSnapshotAsync()
        {
            // get latest snapshot
            var latestSnapshot = await _dbContext.CatalogSnapshots.OrderByDescending(s => s.Imported).FirstOrDefaultAsync();

            if (latestSnapshot == null)
            {
                throw new CatalogSnapshotException("No snapshots found");
            }

            if (latestSnapshot.Published.HasValue)
            {
                throw new CatalogSnapshotException(latestSnapshot.Id, "The latest snapshot has already been published");
            }

            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(latestSnapshot.MachineJsonResponse));
            using var mgs = new MemoryStream(Encoding.UTF8.GetBytes(latestSnapshot.MachineGroupJsonResponse));
            var machines = await JsonSerializer.DeserializeAsync<IEnumerable<Machine>>(ms);
            var machineGroups = await JsonSerializer.DeserializeAsync<IEnumerable<MachineGroup>>(mgs);

            if (machines is null || machineGroups is null)
                throw new Exception("Machine snapshot JSON could not be parsed.");
            await LoadMachineGroupsAsync(machineGroups);

            // manufacturers
            var manufacturers = machines
                .Where(m => m.Manufacturer != null)
                .Select(m => m.Manufacturer).Distinct(new ManufacturerComparer()).ToList();
            await LoadManufacturersAsync(manufacturers);

            // machines
            await LoadPinballMachinesAsync(machines);

            // keywords
            await LoadKeywordsAsync(machines);

            // keyword mappings
            await MapPinballKeywordsAsync(machines);

            // mark snapshot as published
            latestSnapshot.Published = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            // get results
            var machineTotal = await _dbContext.PinballMachines.CountAsync();
            var machineGroupsTotal = await _dbContext.PinballMachineGroups.CountAsync();
            var manufacturersTotal = await _dbContext.PinballManufacturers.CountAsync();
            var keywordsTotal = await _dbContext.PinballKeywords.CountAsync();

            var result = new CatalogSnapshotPublishResult
            {
                MachineTotal = machineTotal,
                MachineGroupTotal = machineGroupsTotal,
                ManufacturerTotal = manufacturersTotal,
                KeywordTotal = keywordsTotal,
                SnapshotId = latestSnapshot.Id,
                Imported = latestSnapshot.Imported
            };

            return result;
        }

        private async Task LoadMachineGroupsAsync(IEnumerable<MachineGroup> machineGroups)
        {
            // convert collection to db entities
            var dbMachineGroups = machineGroups
                .Distinct(new MachineGroupComparer())
                .Select(m => new PinballMachineGroup
            {
                Id = m.OpdbId.ToString(),
                Name = m.Name,
                ShortName = m.Shortname,
                Description = m.Description
            }).ToList();

            await _dbContext.BulkInsertOrUpdateAsync(dbMachineGroups);
        }

        private async Task LoadManufacturersAsync(IEnumerable<Manufacturer?> manufacturers)
        {
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

            foreach(var newManufacturer in newManufacturers)
            {
                if (savedManufacturers.TryGetValue(newManufacturer.Id, out var savedManufacturer))
                {
                    _dbContext.Entry(savedManufacturer).CurrentValues.SetValues(newManufacturer);
                } 
                else
                {
                    _dbContext.Add(newManufacturer);
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        private async Task LoadPinballMachinesAsync(IEnumerable<Machine> machines)
        {
            var newMachines = machines.Select(m => new PinballMachine
            {
                Id = m.OpdbId.ToString(),
                CommonName = m.CommonName,
                Name = m.Name ?? string.Empty,
                IpdbId = m.IpdbId,
                ManufactureDate = m.ManufactureDate,
                ManufacturerId = m.Manufacturer?.ManufacturerId,
                PlayerCount = (short)m.PlayerCount,
                MachineGroupId = m.OpdbId.GroupString,
                TypeId = m.MachineType
            }).ToList();

            await _dbContext.BulkInsertOrUpdateAsync(newMachines);
        }

        private async Task LoadKeywordsAsync(IEnumerable<Machine> machines)
        {
            var newKeywords = machines.SelectMany(m => m.Keywords ?? new List<string>()).Distinct().ToList();

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

        private async Task MapPinballKeywordsAsync(IEnumerable<Machine> machines)
        {
            var savedKeywords = await _dbContext.PinballKeywords.ToDictionaryAsync(k => k.Name, k => k.Id);

            var keywordMappings = new List<PinballMachineKeywordMapping>();

            foreach(var machine in machines)
            {
                var mappings = machine.Keywords?.Select(kw => new PinballMachineKeywordMapping
                {
                    KeywordId = savedKeywords[kw],
                    MachineId = machine.OpdbId.ToString()
                });
                if (mappings is not null)
                    keywordMappings.AddRange(mappings);
            }

            await _dbContext.BulkInsertOrUpdateAsync(keywordMappings);
        }

        public async Task<IEnumerable<OpdbCatalogSnapshot>> GetAllCatalogSnapshotsAsync()
        {
            var result = await _dbContext.CatalogSnapshots.AsNoTracking().ToListAsync();
            return result;
        }

        public async Task ResetCatalogAsync()
        {
            // unpublish catalogs
            var catalogSnapshots = await _dbContext.CatalogSnapshots.ToListAsync();

            foreach(var snapshot in catalogSnapshots)
            {
                snapshot.Published = null;
            }

            await _dbContext.SaveChangesAsync();

            // delete keywords
            await _dbContext.PinballKeywords.BatchDeleteAsync();

            // delete features
            await _dbContext.PinballFeatures.BatchDeleteAsync();

            // delete pinball machines
            await _dbContext.PinballMachines.BatchDeleteAsync();

            // delete manufacturers
            await _dbContext.PinballManufacturers.BatchDeleteAsync();

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
                if(savedChangelog != null)
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
            var latestSnapshot = await _dbContext.CatalogSnapshots.AsNoTracking().OrderByDescending(s => s.Created).FirstOrDefaultAsync();

            if (latestSnapshot == null)
            {
                throw new InvalidOperationException("No snapshots exist to get machine types for");
            }

            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(latestSnapshot.MachineJsonResponse));
            var machines = await JsonSerializer.DeserializeAsync<IEnumerable<Machine>>(ms);

            if (machines is null) throw new Exception("Machine snapshot JSON could not be parsed");
            var machineTypeGroups = machines.GroupBy(m => m.MachineType).ToDictionary(g => g.Key?.ToString() ?? "Unknown", g => g.Count());

            return machineTypeGroups;
        }

        public async Task<Dictionary<string, int>> GetAllDisplayTypesAsync()
        {
            var latestSnapshot = await _dbContext.CatalogSnapshots.AsNoTracking().OrderByDescending(s => s.Created).FirstOrDefaultAsync();

            if (latestSnapshot == null)
            {
                throw new InvalidOperationException("No snapshots exist to get display types for");
            }

            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(latestSnapshot.MachineJsonResponse));
            var machines = await JsonSerializer.DeserializeAsync<IEnumerable<Machine>>(ms);

            if (machines is null) throw new Exception("Machine snapshot JSON could not be parsed");
            var displayTypeGroups = machines.GroupBy(m => m.Display ?? "null").ToDictionary(g => g.Key.ToString(), g => g.Count());

            return displayTypeGroups;
        }
    }
}