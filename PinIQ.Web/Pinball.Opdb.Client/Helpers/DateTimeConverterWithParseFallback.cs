using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pinball.OpdbClient.Helpers
{
    public class DateTimeConverterWithParseFallback : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TryGetDateTime(out DateTime value))
            {
                return value;
            }

            if (DateTime.TryParse(reader.GetString(), out DateTime value2))
            {
                return value2;
            }

            return DateTime.MinValue;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}