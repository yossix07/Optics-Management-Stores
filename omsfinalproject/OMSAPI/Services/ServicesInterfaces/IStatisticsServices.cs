using OMSAPI.Models.Store;

namespace OMSAPI.Services.ServicesInterfaces
{
    public interface IStatisticsServices
    {
        Dictionary<string, int> GetOrderedProductsAmount(List<Order> orders);

        Dictionary<string, decimal> GetOrderedProductsPrices(List<Order> orders);

        Task<Dictionary<DateOnly, decimal>?> PeriodicOrderMoneyAmout(List<Order> orders);

        Task<Dictionary<DateOnly, int>?> PeriodicOrderAmout(List<Order> orders);

        Task<Dictionary<DateOnly, int>?> PeriodicProductAmout(List<Order> orders, string productId);

        Task<Dictionary<DateOnly, decimal>?> PeriodicProductMoneyAmount(List<Order> orders, string productId);
    }
}
