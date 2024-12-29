using System.Text.Json;
using System.Text.Json.Serialization;

namespace MC_GymMasterWebAPI.DTOs
{
    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if it's an object and read it as a JSON object
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Skip to the actual date field, adjust based on your JSON structure
                reader.Read(); // Move to next token
                reader.Read(); // Move to the "date" token
                string dateString = reader.GetString();
                return DateOnly.FromDateTime(DateTime.Parse(dateString));
            }
            else if (reader.TokenType == JsonTokenType.String)
            {
                return DateOnly.FromDateTime(DateTime.Parse(reader.GetString()));
            }
            else
            {
                throw new JsonException($"Unexpected token type: {reader.TokenType}");
            }
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd")); // Format as needed
        }
    }
}
