using Pinball.Api.Services.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pinball.Entities.Data.Opdb;

namespace Pinball.Api.Services.Interfaces;

public interface IPinballMachineCatalogService
{
    Task<OpdbCatalogSnapshot> ImportNewCatalogSnapshotAsync();
    Task<OpdbCatalogSnapshot?> GetCatalogSnapshotAsync(int id);
    Task<OpdbCatalogSnapshot?> GetPublishedCatalogSnapshotAsync();
    Task<CatalogSnapshotPublishResult> PublishCatalogSnapshotAsync();
    Task DeleteCatalogSnapshotAsync(int id);
    Task<IEnumerable<OpdbCatalogSnapshot>> GetAllCatalogSnapshotsAsync();

    Task<int> UpdateChangelogsAsync();
    Task<IEnumerable<OpdbChangelog>> GetChangelogsAsync();

    Task<Dictionary<string, int>> GetAllMachineTypesAsync();
    Task<Dictionary<string, int>> GetAllDisplayTypesAsync();
    Task ResetCatalogAsync();
}