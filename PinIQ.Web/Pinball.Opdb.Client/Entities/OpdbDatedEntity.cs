using Pinball.OpdbClient.Helpers;
using System;
using System.Text.Json.Serialization;

namespace Pinball.OpdbClient.Entities
{
    public abstract class OpdbDatedEntity : OpdbEntity
    {
        [JsonPropertyName("created_at"), JsonConverter(typeof(DateTimeConverterWithParseFallback))]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at"), JsonConverter(typeof(DateTimeConverterWithParseFallback))]
        public DateTime UpdatedAt { get; set; }
    }
}
