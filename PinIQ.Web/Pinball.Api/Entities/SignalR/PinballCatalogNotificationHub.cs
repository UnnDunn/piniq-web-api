using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Pinball.Entities.Api.Responses.PinballCatalog;
using CatalogSnapshotPublishResult = Pinball.Api.Services.Entities.CatalogSnapshotPublishResult;

namespace Pinball.Api.Entities.SignalR;

public interface IPinballCatalogNotificationClient
{
    Task AddCatalogSnapshot(CatalogSnapshot catalogSnapshot);
    Task PublishCatalogSnapshot(CatalogSnapshotPublishResult catalogSnapshotPublishResult);
    Task RemoveCatalogSnapshot(int id);
}

[AllowAnonymous]
public class PinballCatalogNotificationHub : Hub<IPinballCatalogNotificationClient>
{
}