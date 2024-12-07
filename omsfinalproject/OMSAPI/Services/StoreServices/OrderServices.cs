using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OMSAPI.Models.Store;
using OMSAPI.Services.ServicesInterfaces;

namespace OMSAPI.Services.StoreServices
{
    
    public class OrderServices : IOrderServices
    {

        private readonly ILogger<OrderServices> _logger;
        private readonly IDatabaseServices _databaseServices;
        private readonly string ordersCollectionName = General.Constants.ordersCollectionName;

        public OrderServices(ILogger<OrderServices> logger, IDatabaseServices databaseServices)
        {
            _logger = logger;
            _databaseServices = databaseServices;
        }

        public async Task<List<Order>?> GetAllOrders(string dbName, string status)
        {
            var collection = _databaseServices.FindCollectionByDB<Order>(dbName, ordersCollectionName);
            if (collection != null)
            {
                var list = await collection.Find(order => order.Status == status).ToListAsync();
                if (list != null)
                {
                    _logger.LogInformation($"GetAllOrders finished successfully for {dbName}");
                    return list;
                }
            }

            _logger.LogError($"The collection {ordersCollectionName} was not found in the database {dbName}");
            return null;
        }


        public async Task<Order?> GetOrderById(string dbName, string orderId)
        {

            var collection = _databaseServices.FindCollectionByDB<Order>(dbName, ordersCollectionName);

            if (collection != null)
            {
                var product = await collection.Find(order => order.Id == orderId).FirstOrDefaultAsync();
                if (product != null)
                {
                    _logger.LogInformation($"GetOrderById finished successfully for {dbName}");
                    return product;
                }
            }
            _logger.LogError($"The collection {ordersCollectionName} was not found in the database {dbName}");
            return null;
        }

        public async Task<List<Order>?> GetOrderByUser(string dbName, string userId, string status)
        {

            var collection = _databaseServices.FindCollectionByDB<Order>(dbName, ordersCollectionName);

            if (collection != null)
            {
                // find all orders of the user and return all of them
                var product = await collection.Find(order => order.UserId == userId && order.Status == status).ToListAsync();
                if (product != null)
                {
                    _logger.LogInformation($"GetOrderByUser finished successfully for {dbName}");
                    return product;
                }
            }
            _logger.LogError($"The collection {ordersCollectionName} was not found in the database {dbName}");
            return null;
        }


        public async Task<ActionResult<Order>?> CreateOrder(string dbName, Order order)
        {
            try
            {
                var collection = _databaseServices.FindCollectionByDB<Order>(dbName, ordersCollectionName);

                if (collection != null)
                {
                    await collection.InsertOneAsync(order);
                    _logger.LogInformation($"Order {order.ToString()} created for tenant {dbName}");
                    return order;
                }
                _logger.LogError($"The collection {ordersCollectionName} was not found in the database {dbName}");
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<ActionResult<Order>?> UpdateOrder(string dbName, string orderId, Order newOrder)
        {
            try
            {
                var productCollection = _databaseServices.FindCollectionByDB<Order>(dbName, ordersCollectionName);

                newOrder.Id = orderId;
                if (productCollection != null)
                {
                    await productCollection.ReplaceOneAsync(p => p.Id == orderId, newOrder);
                    _logger.LogInformation($"Product {newOrder.ToString()} updated for tenant {dbName}");
                    return newOrder;
                }
                _logger.LogError($"The collection {ordersCollectionName} was not found in the database {dbName}");
                return null;
            }
            catch
            {
                return null;
            }
        }


        public async Task<ActionResult<Order>?> DeleteOrder(string dbName, Order order)
        {
            try
            {
                var collection = _databaseServices.FindCollectionByDB<Product>(dbName, ordersCollectionName);

                if (collection != null)
                {
                    await collection.DeleteOneAsync(p => p.Id == order.Id);
                    _logger.LogInformation($"Order {order.ToString()} deleted for tenant {dbName}");
                    return order;
                }
                _logger.LogError($"The collection {ordersCollectionName} was not found in the database {dbName}");
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
    
}
