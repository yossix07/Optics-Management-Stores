using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OMSAPI.Models.Entities
{
    public class User : IEntity
    {
        [BsonId]
        [Required]
        [MaxLength(9)]
        [MinLength(9)]
        [BsonElement("id")]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [Required]
        [BsonElement("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [BsonElement("email")]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [Phone]
        [BsonElement("phoneNumber")]
        [JsonPropertyName("phoneNumber")]

        public string PhoneNumber { get; set; }

        [BsonElement("passwordHash")]
        [BsonRepresentation(BsonType.Binary)]
        public byte[] PasswordHash { get; set; }

        [BsonElement("passwordSalt")]
        [BsonRepresentation(BsonType.Binary)]
        public byte[] PasswordSalt { get; set; }

        [BsonElement("dateOfBirth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

    }
}
