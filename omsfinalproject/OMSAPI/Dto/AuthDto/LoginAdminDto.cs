using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OMSAPI.Dto.AuthDto
{
    public class LoginAdminDto
    {
        [Required]
        [MinLength(2, ErrorMessage = "Name must be at least 3 characters long.")]
        [RegularExpression("^[a-zA-Z][a-zA-Z0-9]*$", ErrorMessage = "Name must contain at least one letter and can only contain letters and numbers.")]
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("password")]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d).{5,}$", ErrorMessage = "Password must contain at least one letter and one digit, and be at least 5 characters long.")]
        public string Password { get; set; } = string.Empty;
    }
}
