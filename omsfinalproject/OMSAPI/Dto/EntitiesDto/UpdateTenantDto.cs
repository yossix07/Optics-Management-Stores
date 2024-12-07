using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OMSAPI.Dto.EntitiesDto
{
    public class UpdateTenantDto
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; } = null;

        [JsonPropertyName("email")]
        public string? Email { get; set; } = null;

        [Phone]
        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; set; } = null;

        [JsonPropertyName("address")]
        public string? Address { get; set; } = null;
    }
}
