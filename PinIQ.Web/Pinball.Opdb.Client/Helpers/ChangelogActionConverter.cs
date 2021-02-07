using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pinball.OpdbClient.Helpers
{
    public class ChangelogActionConverter : JsonConverter<Entities.Action>
    {
        public override Entities.Action Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var stringValue = reader.GetString();
            Entities.Action result;
            switch (stringValue)
            {
                case "delete":
                    result = Entities.Action.Delete;
                    break;
                default:
                    result = Entities.Action.Move;
                    break;
            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, Entities.Action value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}