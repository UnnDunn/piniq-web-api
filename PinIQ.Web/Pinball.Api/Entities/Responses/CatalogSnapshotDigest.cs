using Pinball.OpdbClient.Entities;
using Pinball.OpdbClient.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Pinball.Entities.Api.Responses.PinballCatalog;
using Pinball.Entities.Data.Opdb;

namespace Pinball.Api.Entities.Responses
{
    public static class CatalogSnapshotDigestExtensions
    {
        public static async Task<CatalogSnapshotDigest> ToCatalogSnapshotDigestAsync(this OpdbCatalogSnapshot o)
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
                    Created = o.Created,
                    Updated = o.Updated,
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
