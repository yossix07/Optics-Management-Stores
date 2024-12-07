using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OMSAPI.General
{ 
    public sealed class TimeOnlyConverter : JsonConverter<TimeOnly>
    {
        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return TimeOnly.Parse(reader.GetString()!);
        }

        public override TimeOnly ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return TimeOnly.Parse(reader.GetString()!);
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        {
            var formattedTime = value.ToString("HH:mm");
            writer.WriteStringValue(formattedTime);
        }

        public override void WriteAsPropertyName(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        {
            var formattedTime = value.ToString("HH:mm");
            writer.WriteStringValue(formattedTime);
        }
    }  

}
