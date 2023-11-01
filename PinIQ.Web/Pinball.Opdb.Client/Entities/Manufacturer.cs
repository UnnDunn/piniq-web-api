using System.Text.Json.Serialization;

namespace Pinball.OpdbClient.Entities
{
    public class Manufacturer : OpdbDatedEntity
    {
        [JsonPropertyName("manufacturer_id")]
        public int ManufacturerId { get; set; }
        [JsonPropertyName("name")] 
        public string Name { get; set; } = null!;
        [JsonPropertyName("full_name")]
        public string? FullName { get; set; }
    }
}
