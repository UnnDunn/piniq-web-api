using Pinball.OpdbClient.Entities;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pinball.OpdbClient.Helpers
{
    public class MachineTypeConverter : JsonConverter<MachineType>
    {
        public override MachineType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var stringValue = reader.GetString();
            MachineType result;
            switch (stringValue)
            {
                case "me":
                    result = MachineType.Mechanichal;
                    break;
                case "em":
                    result = MachineType.ElectroMechanical;
                    break;
                case "ss":
                    result = MachineType.SolidState;
                    break;
                default:
                    result = MachineType.None;
                    break;
            }
            return result;
        }

        public override void Write(Utf8JsonWriter writer, MachineType value, JsonSerializerOptions options)
        {
            string stringValue = string.Empty;
            switch (value)
            {
                case MachineType.SolidState:
                    stringValue = "ss";
                    break;
                case MachineType.ElectroMechanical:
                    stringValue = "em";
                    break;
                case MachineType.Mechanichal:
                    stringValue = "me";
                    break;
                default:
                    break;
            }

            writer.WriteStringValue(stringValue);
        }
    }
}