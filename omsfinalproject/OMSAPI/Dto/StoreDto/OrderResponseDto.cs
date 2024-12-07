using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using OMSAPI.Models.Store;
using OMSAPI.Models.Entities;

namespace OMSAPI.Dto.StoreDto
{
    public class OrderResponseDto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("orderDate")]
        public DateTime OrderDate { get; set; }

        [BsonElement("userId")]
        public string UserId { get; set; }

        [BsonElement("userName")]
        public string UserName { get; set; }

        [BsonElement("userEmail")]
        public string UserEmail { get; set; }

        [BsonElement("userPhoneNumber")]
        public string UserPhoneNumber { get; set; }

        [BsonElement("orderItems")]
        public List<OrderItem> OrderItems { get; set; }

        [BsonElement("totalPrice")]
        public decimal TotalPrice { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }

        public OrderResponseDto(string userId, string username, string userEmail, string userPhoneNumber, List<OrderItem> orderItems)
        {
            Id = ObjectId.GenerateNewId().ToString();
            OrderDate = DateTime.UtcNow;
            UserId = userId;
            UserName = username;
            UserEmail = userEmail;
            UserPhoneNumber = userPhoneNumber;
            OrderItems = orderItems;
            TotalPrice = orderItems.Sum(item => item.TotalPrice);
            Status = OrderStatus.Pending.ToString();
        }

        public OrderResponseDto(Order order, User user)
        {
            Id = order.Id;
            OrderDate = order.OrderDate;
            UserId = order.UserId;
            UserName = user.Name;
            UserEmail = user.Email;
            UserPhoneNumber = user.PhoneNumber;
            OrderItems = order.OrderItems;
            TotalPrice = order.TotalPrice;
            Status = order.Status;
        }
    }
}
