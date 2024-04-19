using System;

namespace Pinball.Api.Services.Entities;

public record CatalogSnapshotPublishResult
{
    public int MachineTotal { get; set; }
    public int MachineGroupTotal { get; set; }
    public int ManufacturerTotal { get; set; }
    public int KeywordTotal { get; set; }
    public int SnapshotId { get; set; }
    public DateTimeOffset Imported { get; set; }
}

