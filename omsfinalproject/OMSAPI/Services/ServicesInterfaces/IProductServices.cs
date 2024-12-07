using Microsoft.AspNetCore.Mvc;
using OMSAPI.Models.Store;

namespace OMSAPI.Services.ServicesInterfaces
{
    public interface IProductServices
    {
        Task<List<Product>?> GetAllProducts(string dbName);
        Task<Product?> GetProduct(string dbName, string productId);
        Task<Product?> CreateProduct(string dbName, Product product);
        Task<Product?> UpdateProduct(string dbName, string productId, Product newProduct);

        Task<Product?> DeleteProduct(string dbName, Product product);

        Task<bool> SubstractQuantity(string tenantId, ShoppingCart cart);

        Task<bool> AddQuantity(string tenantId, Order order);


    }
}
