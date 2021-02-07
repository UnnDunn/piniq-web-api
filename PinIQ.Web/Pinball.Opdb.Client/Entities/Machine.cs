using Pinball.OpdbClient.Helpers;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Pinball.OpdbClient.Entities
{
    public class Machine : MachineGroup
    {
        [JsonPropertyName("common_name")]
        public string? CommonName { get; set; }
        [JsonPropertyName("physical_machine")]
        public int PhysicalMachine { get; set; }
        [JsonPropertyName("ipdb_id")]
        public int? IpdbId { get; set; }
        [JsonPropertyName("type")]
        public string? MachineType { get; set; }
        [JsonPropertyName("display")]
        public string? Display { get; set; }
        [JsonPropertyName("manufacture_date"), JsonConverter(typeof(NullableDateTimeConverterWithParseFallback))]
        public DateTime? ManufactureDate { get; set; }
        [JsonPropertyName("manufacturer")]
        public Manufacturer? Manufacturer { get; set; }
        [JsonPropertyName("player_count")]
        public int PlayerCount { get; set; }
        [JsonPropertyName("keywords")]
        public List<string>? Keywords { get; set; }
        [JsonPropertyName("features")]
        public List<string>? Features { get; set; }
    }
}
