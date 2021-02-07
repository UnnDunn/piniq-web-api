using Pinball.OpdbClient.Helpers;
using System.Text.Json.Serialization;

namespace Pinball.OpdbClient.Entities
{
    public class Changelog : OpdbDatedEntity
    {
        [JsonPropertyName("changelog_id")]
        public int ChangelogId { get; set; }
        [JsonPropertyName("opdb_id_deleted"), JsonConverter(typeof(OpdbIdConverter))]
        public OpdbId? OpdbIdDeleted { get; set; }
        [JsonConverter(typeof(ChangelogActionConverter)), JsonPropertyName("action")]
        public Action Action { get; set; }
        [JsonPropertyName("opdb_id_replacement"), JsonConverter(typeof(OpdbIdConverter))]
        public OpdbId? OpdbIdReplacement { get; set; }
    }
}
