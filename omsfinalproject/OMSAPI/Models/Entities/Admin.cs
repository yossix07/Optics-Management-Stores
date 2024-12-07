using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace OMSAPI.Models.Entities
{
    public class Admin : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [BsonElement("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [EmailAddress]
        [BsonElement("email")]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [BsonElement("passwordHash")]
        [BsonRepresentation(BsonType.Binary)]
        public byte[] PasswordHash { get; set; }

        [BsonElement("passwordSalt")]
        [BsonRepresentation(BsonType.Binary)]
        public byte[] PasswordSalt { get; set; }
    }
}
