using Pinball.Api.Data.Entities;
using Pinball.Api.Helpers;
using Pinball.OpdbClient.Entities;
using Pinball.OpdbClient.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pinball.Api.Entities.Responses
{
    public class CatalogSnapshotDigest
    {
        public int Id { get; set; }
        public DateTime Imported { get; set; }
        public DateTime? Published { get; set; }
        public int MachineCount { get; set; }
        public int MachineGroupCount { get; set; }
        public int ManufacturerCount { get; set; }
        public int KeywordCount { get; set; }

        public static async Task<CatalogSnapshotDigest> FromSnapshot(OpdbCatalogSnapshot o)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(o.MachineJsonResponse)))
            using (var mgs = new MemoryStream(Encoding.UTF8.GetBytes(o.MachineGroupJsonResponse)))
            {
                var jsonOptions = new JsonSerializerOptions();
                jsonOptions.Converters.Add(new NullableDateTimeConverterWithParseFallback());

                var machines = await JsonSerializer.DeserializeAsync<IEnumerable<Machine>>(ms, jsonOptions);
                var machineGroups = await JsonSerializer.DeserializeAsync<IEnumerable<MachineGroup>>(mgs, jsonOptions);

                if (machines is null || machineGroups is null)
                    throw new Exception("Parsing machines or machine groups from Opdb failed");
                var result = new CatalogSnapshotDigest
                {
                    Id = o.Id,
                    Imported = o.Imported,
                    Published = o.Published,
                    MachineCount = machines.Count(),
                    MachineGroupCount = machineGroups.Count(),
                    ManufacturerCount = machines
                        .Where(m => m.Manufacturer != null)
                        .Select(m => m.Manufacturer?.ManufacturerId)
                        .Distinct()
                        .Count(),
                    KeywordCount = machines.SelectMany(m => m.Keywords ?? new List<string>()).Distinct().Count()
                };

                return result;
            }
        }
    }
}
