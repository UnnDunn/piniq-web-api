using System;
using System.Collections;

namespace Pinball.Api.Data.Entities
{
    public class OpdbCatalogSnapshot : TimedEntity
    {
        public int Id { get; set; }
        public DateTime Imported { get; set; }
        public DateTime? Published { get; set; }
        public string MachineJsonResponse { get; set; } = null!;
        public string MachineGroupJsonResponse { get; set; } = null!;
    }

    public class OpdbChangelog
    {
        public int Id { get; set; }
        public string OpdbId { get; set; } = null!;
        public string? NewOpdbId { get; set; }
        public OpdbChangelogAction Action { get; set; }
        public DateTime Date { get; set; }
    }

    public enum OpdbChangelogAction { Move, Delete };
}
