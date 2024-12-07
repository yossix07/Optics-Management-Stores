using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using OMSAPI.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace OMSAPI.Models.Store
{
    public class ShoppingCart
    {
        [Required]
        [BsonElement("userId")]
        public string UserId { get; set; }

        [Required]
        [BsonElement("orderItems")]
        public List<OrderItem> CartItems { get; set; }


        public ShoppingCart(string userId, List<OrderItem> cartItems)
        {
            UserId = userId;
            CartItems = cartItems;
        }

        public Order CreateOrderFromShoppingCart(User user)
        {
            return new Order(UserId, CartItems);
        }
    }
}
