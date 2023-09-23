using Pinball.OpdbClient.Helpers;
using System.Text.Json.Serialization;

namespace Pinball.OpdbClient.Entities
{
    public class MachineGroup : OpdbDatedEntity
    {
        [JsonPropertyName("opdb_id"), JsonConverter(typeof(OpdbIdConverter))]
        public OpdbId OpdbId { get; set; } = null!;
        [JsonPropertyName("name")] 
        public string Name { get; set; } = null!;
        [JsonPropertyName("shortname")] 
        public string? Shortname { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}
