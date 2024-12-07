using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OMSAPI.Models.Entities
{
    public class ResetPasswordModel
    {
        [JsonIgnore]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [Required, EmailAddress]
        [BsonElement("email")]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [Required]
        [BsonElement("VerificationCode")]
        public string VerificationCode { get; set; }

        [Required]
        [BsonElement("VarificationCodeExpired")]
        public DateTime VarificationCodeExpired{ get; set; }

        [Required]
        public string Role { get; set; }
    }
}
