using System.ComponentModel.DataAnnotations;

namespace OMSAPI.Dto
{
    public class ResetPasswordDto
    {
        [Required,EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
