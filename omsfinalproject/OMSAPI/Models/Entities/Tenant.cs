using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Attributes;
using OMSAPI.Models.Appointments;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OMSAPI.Models.Entities
{
    [BsonIgnoreExtraElements]
    public class Tenant : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [EmailAddress]
        [BsonElement("email")]
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("passwordHash")]
        [BsonRepresentation(BsonType.Binary)]
        public byte[] PasswordHash { get; set; }

        [BsonElement("passwordSalt")]
        [BsonRepresentation(BsonType.Binary)]
        public byte[] PasswordSalt { get; set; }

        public AppointmentSettings appointmentSettings { get; set; }

        [Phone]
        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;

        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;

    }
}
