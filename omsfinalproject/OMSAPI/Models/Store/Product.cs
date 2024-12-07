using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Xml.Linq;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace OMSAPI.Models.Store
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        [SwaggerSchema(ReadOnly = true)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("name")]
        [Required]
        public string Name { get; set; }

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("imageUrl")]
        public string? Image { get; set; }

        [BsonElement("price")]
        [Required]
        public decimal Price { get; set; }

        [BsonElement("stock")]
        public int Stock { get; set; } = 0;



        public override string ToString()
        {
            return $"Product {{ Id = {Id}, Name = {Name} }}";
        }
    }


   
}
