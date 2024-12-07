using System.Text.Json;
using System.Text.Json.Serialization;

public class TimeSpanConverter : JsonConverter<TimeSpan>
{
    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jsonObject = JsonSerializer.Deserialize<JsonElement>(ref reader);
        //int days = jsonObject.GetProperty("days").GetInt32();
        int hours = jsonObject.GetProperty("hours").GetInt32();
        int minutes = jsonObject.GetProperty("minutes").GetInt32();
        int seconds = jsonObject.GetProperty("seconds").GetInt32();
        //int milliseconds = jsonObject.GetProperty("milliseconds").GetInt32();
        return new TimeSpan(hours, minutes, seconds);
    }


    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        //writer.WriteNumber("days", value.Days);
        writer.WriteNumber("hours", value.Hours);
        writer.WriteNumber("minutes", value.Minutes);
        writer.WriteNumber("seconds", value.Seconds);
        /* insert any needed properties here */
        writer.WriteEndObject();
    }

}