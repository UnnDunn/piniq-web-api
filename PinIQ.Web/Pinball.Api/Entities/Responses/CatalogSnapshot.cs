using Newtonsoft.Json;
using Pinball.Api.Data.Entities;
using System;
using System.Collections;

namespace Pinball.Api.Entities.Responses
{
    public class CatalogSnapshot
    {
        public int Id { get; set; }
        public DateTime Imported { get; set; }
        public DateTime? Published { get; set; }
        public string MachineJson { get; set; }
        public string MachineGroupJson { get; set; }

        public static implicit operator CatalogSnapshot(OpdbCatalogSnapshot o)
        {
            return new CatalogSnapshot
            {
                Id = o.Id,
                Imported = o.Imported,
                Published = o.Published,
                MachineJson = o.MachineJsonResponse,
                MachineGroupJson = o.MachineGroupJsonResponse
            };
        }
    }

}
