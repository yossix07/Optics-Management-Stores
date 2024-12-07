using MongoDB.Bson.Serialization.IdGenerators;
using System.ComponentModel.DataAnnotations;

namespace OMSAPI.Dto
{
    public class VerifyCodeRequestDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string VerificationCode { get; set; }

        [Required, RegularExpression("^[a-zA-Z][a-zA-Z0-9]*$", ErrorMessage = "Name must contain at least one letter and can only contain letters and numbers.")]
        public string NewPassword { get; set; }
    }
}
