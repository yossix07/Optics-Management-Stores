using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OMSAPI.Dto.AuthDto
{
    public class LoginUserDto
    {
        [Required]
        [MaxLength(9)]
        [MinLength(9)]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Id must contain only numbers")]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [Required]
        [JsonPropertyName("password")]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d).{5,}$", ErrorMessage = "Password must contain at least one letter and one digit, and be at least 5 characters long.")]
        public string Password { get; set; }

    }
}
