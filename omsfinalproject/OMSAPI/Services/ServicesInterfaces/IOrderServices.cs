using Microsoft.AspNetCore.Mvc;
using OMSAPI.Models.Store;

namespace OMSAPI.Services.ServicesInterfaces
{
    public interface IOrderServices
    {
        Task<List<Order>?> GetAllOrders(string dbName, string status);
        Task<Order?> GetOrderById(string dbName, string orderId);
        Task<List<Order>?> GetOrderByUser(string dbName, string userId, string status);
        Task<ActionResult<Order>?> CreateOrder(string dbName, Order order);
        Task<ActionResult<Order>?> UpdateOrder(string dbName, string orderId, Order newOrder);
        Task<ActionResult<Order>?> DeleteOrder(string dbName, Order order);

    }
}
