using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using OMSAPI.Dto.StoreDto;
using Newtonsoft.Json;

namespace OMSAPI.Models.Store
{

    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("orderDate")]
        public DateTime OrderDate { get; set; }

        [BsonElement("userId")]
        public string UserId { get; set; }

        [BsonElement("orderItems")]
        public List<OrderItem> OrderItems { get; set; }

        [BsonElement("totalPrice")]
        public decimal TotalPrice { get; set; }

        [BsonElement("status")]
        public string Status{ get; set;}


        public Order(string userId, List<OrderItem> orderItems)
        {
            Id = ObjectId.GenerateNewId().ToString();
            OrderDate= DateTime.UtcNow;
            UserId = userId;
            OrderItems = orderItems;
            TotalPrice = orderItems.Sum(item => item.TotalPrice);
            Status = OrderStatus.Pending.ToString(); 
        }

        public override string? ToString()
        {
            return $"[Order Id: {Id}, Status: {Status}]";
        }
    }
}
