using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace OMSAPI.Models.Store
{
    public class OrderItem
    {
        [Required]
        [BsonElement("productId")]
        public string ProductId { get; set; }

        [Required]
        [BsonElement("name")]
        public string Name { get; set; }

        [Required]
        [BsonElement("quantity")]
        public int Quantity { get; set; }

        
        [Required]
        [BsonElement("price")]
        public decimal Price { get; set; }

        [Required]
        [BsonIgnoreIfDefault]
        [BsonElement("totalPrice")]
        public decimal TotalPrice { get { return Quantity * Price; } }
    }
}
