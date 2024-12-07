using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OMSAPI.Dto.EntitiesDto
{
    public class UserDto
    {

        [Required]
        [MaxLength(9)]
        [MinLength(9)]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Id must contain only numbers")]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "Name must be at least 3 characters long.")]
        [RegularExpression("^[a-zA-Z][a-zA-Z0-9]*$", ErrorMessage = "Name must contain at least one letter and can only contain letters and numbers.")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [Required]
        [JsonPropertyName("password")]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d).{5,}$", ErrorMessage = "Password must contain at least one letter and one digit, and be at least 5 characters long.")]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [Required]
        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }
    }
}

