using System.Collections.Generic;
using System.Threading.Tasks;
using Pinball.Entities.Api.Responses.PinballCatalog;
using Pinball.Entities.Data.Opdb;
using CatalogSnapshotPublishResult = Pinball.Api.Services.Entities.CatalogSnapshotPublishResult;

namespace Pinball.Api.Services.Interfaces;

public interface IPinballMachineCatalogService
{
    Task<CatalogSnapshot> ImportNewCatalogSnapshotAsync();
    Task<CatalogSnapshot?> GetCatalogSnapshotAsync(int id);
    Task<CatalogSnapshot?> GetPublishedCatalogSnapshotAsync();
    Task<CatalogSnapshotPublishResult> PublishCatalogSnapshotAsync(int? id = null);
    Task DeleteCatalogSnapshotAsync(int id);
    Task<IEnumerable<CatalogSnapshot>> GetAllCatalogSnapshotsAsync();

    Task<int> UpdateChangelogsAsync();
    Task<IEnumerable<OpdbChangelog>> GetChangelogsAsync();

    Task<Dictionary<string, int>> GetAllMachineTypesAsync();
    Task<Dictionary<string, int>> GetAllDisplayTypesAsync();
    Task ResetCatalogAsync();
    Task<List<CatalogSnapshot>> GetCatalogSnapshotsAsync(IEnumerable<int> ids);
    Task RefreshCatalogSnapshotsAsync();
    Task RefreshCatalogSnapshotAsync(int id);
}