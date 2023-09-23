using Pinball.OpdbClient.Entities;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pinball.OpdbClient.Helpers
{
    public class OpdbIdConverter : JsonConverter<OpdbId?>
    {
        public override OpdbId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetString() ?? null;
        }

        public override void Write(Utf8JsonWriter writer, OpdbId? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value?.ToString());
        }
    }
}