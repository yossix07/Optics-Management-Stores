using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OMSAPI.Dto.EntitiesDto
{
    public class UpdateUserDto
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; } = null;

        [JsonPropertyName("email")]
        public string? Email { get; set; } = null;

        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
