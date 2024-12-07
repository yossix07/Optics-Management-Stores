using MailKit.Search;
using OMSAPI.Models.Store;
using OMSAPI.Services.ServicesInterfaces;
using System.Text.Json;


namespace OMSAPI.Services
{
    public class StatisticsServices : IStatisticsServices
    {

        private readonly ILogger<StatisticsServices> _logger;
        private readonly IProductServices _productServices;
        private readonly IOrderServices _orderServices;
        private readonly PredictionServices _predictionServices;

        public StatisticsServices(ILogger<StatisticsServices> logger, IProductServices productServices, IOrderServices orderServices,PredictionServices predictionServices )
        {
            _logger = logger;
            _productServices = productServices;
            _orderServices = orderServices;
            _predictionServices = predictionServices;
        }


       /// <summary>
       /// The function returns a list of orders that were made in the given date range
       /// </summary>
        public Dictionary<string, int> GetOrderedProductsAmount(List<Order> orders)
        {
            Dictionary<string, int> orderedProducts = new Dictionary<string, int>();
            foreach (var order in orders)
            {
                foreach (var item in order.OrderItems)
                {
                    if (orderedProducts.ContainsKey(item.Name))
                    {
                        orderedProducts[item.Name] += item.Quantity;
                    }
                    else
                    {
                        orderedProducts.Add(item.Name, item.Quantity);
                    }
                }
            }

            // Sort the map by the value (amount of sales)
            orderedProducts = orderedProducts.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            return orderedProducts;
        }

        /// <summary>
        /// The function returns a list of orders that were made in the given date range, each procut multiplied by its price
        /// </summary>
        public Dictionary<string, decimal> GetOrderedProductsPrices(List<Order> orders)
        {
            Dictionary<string, decimal> orderedProducts = new Dictionary<string, decimal>();
            foreach (var order in orders)
            {
                foreach (var item in order.OrderItems)
                {
                    if (orderedProducts.ContainsKey(item.Name))
                    {
                        orderedProducts[item.Name] += item.Quantity * item.Price;
                    }
                    else
                    {
                        orderedProducts.Add(item.Name, item.Quantity * item.Price);
                    }
                }
            }

            // Sort the map by the value (amount of sales)
            orderedProducts = orderedProducts.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            return orderedProducts;
        }

        public async Task<Dictionary<DateOnly, decimal>?> PeriodicOrderMoneyAmout(List<Order> orders)
        {
            var monthlyOrders = SeparateOrdersByMonth(orders);

            // sum all orders in each month
            Dictionary<DateOnly, decimal> monthlyOrderMoneyAmount = new Dictionary<DateOnly, decimal>();
            foreach (var item in monthlyOrders)
            {
                decimal sum = 0;
                foreach (var order in item.Value)
                {
                    sum += order.TotalPrice;
                }
                monthlyOrderMoneyAmount.Add(item.Key, sum);
            }

            var prediction = await Predict<decimal>(monthlyOrderMoneyAmount);
            if (prediction != null)
            {
                return prediction;
            }
            return await ReturnSortDict(monthlyOrderMoneyAmount);
        }

        public async Task<Dictionary<DateOnly, int>?> PeriodicOrderAmout(List<Order> orders)
        {
            var monthlyOrders = SeparateOrdersByMonth(orders);

            // sum all orders in each month
            Dictionary<DateOnly, int> monthlyOrderAmount = new Dictionary<DateOnly, int>();
            foreach (var item in monthlyOrders)
            {
                int sum = 0;
                foreach (var order in item.Value)
                {
                    sum += 1;
                }
                monthlyOrderAmount.Add(item.Key, sum);
            }

            var prediction = await Predict<int>(monthlyOrderAmount);
            if (prediction != null)
            {
                return prediction;
            }
            return await ReturnSortDict(monthlyOrderAmount);
        }

        public async Task<Dictionary<DateOnly, int>?> PeriodicProductAmout(List<Order> orders, string productId)
        {
            var monthlyOrders = SeparateOrdersByMonth(orders);

            // sum all orders in each month
            Dictionary<DateOnly, int> monthlyAmount = new Dictionary<DateOnly, int>();
            foreach (var item in monthlyOrders)
            {
                int sum = 0;
                foreach (var order in item.Value)
                {
                    foreach(var product in order.OrderItems)
                    {
                        if (product.ProductId == productId)
                        {
                            sum += product.Quantity;
                        }
                    }
                }
                monthlyAmount.Add(item.Key, sum);
            }

            var prediction = await Predict<int>(monthlyAmount);
            if (prediction != null)
            {
                return prediction;
            }
            return await ReturnSortDict(monthlyAmount);
        }

        public async Task<Dictionary<DateOnly, decimal>?> PeriodicProductMoneyAmount(List<Order> orders, string productId)
        {
            var monthlyOrders = SeparateOrdersByMonth(orders);

            // sum all orders in each month
            Dictionary<DateOnly, decimal> monthlyMoneyAmount = new Dictionary<DateOnly, decimal>();
            foreach (var item in monthlyOrders)
            {
                decimal sum = 0;
                foreach (var order in item.Value)
                {
                    foreach (var product in order.OrderItems)
                    {
                        if (product.ProductId == productId)
                        {
                            sum += (product.Quantity * product.Price);
                        }
                    }
                }
                monthlyMoneyAmount.Add(item.Key, sum);
            }

            var prediction = await Predict<decimal>(monthlyMoneyAmount);
            if (prediction != null)
            {
                return prediction;
            }
            return await ReturnSortDict(monthlyMoneyAmount);
        }

        private Dictionary<DateOnly, List<Order>> SeparateOrdersByMonth(List<Order> orders)
        {
            // add all orders to the dictionary by month, key should be the yyyy:mm:01
            Dictionary<DateOnly, List<Order>> monthlyOrders = new Dictionary<DateOnly, List<Order>>();
            foreach (var order in orders)
            {
                DateOnly date = new DateOnly(order.OrderDate.Year, order.OrderDate.Month, 1);
                if (monthlyOrders.ContainsKey(date))
                {
                    monthlyOrders[date].Add(order);
                }
                else
                {
                    monthlyOrders.Add(date, new List<Order>() { order });
                }
            }
            return monthlyOrders;
        }

        private async Task<Dictionary<DateOnly, T>?> ReturnSortDict<T>(Dictionary<DateOnly, T> dict)
        {
            return await Task.Run(() =>
            {
                var orderedDict = dict.OrderBy(x => x.Key)
                                                    .ToDictionary(x => x.Key, x => x.Value);
                return orderedDict;
            });
        }

        private async Task<Dictionary<DateOnly, T>?> Predict<T>(Dictionary<DateOnly, T> dict)
        {
            var json = JsonSerializer.Serialize(dict);
            var prediction = await _predictionServices.UsePredictionModel(json);
            if (prediction == null)
            {
                return null;
            }
            else
            {
                // add prediction to the dictionary
                DateOnly lastMonth = dict.Keys.Max();
                T value = (T)Convert.ChangeType(prediction, typeof(T)); // use Convert.ChangeType method to cast the data type
                dict.Add(lastMonth.AddMonths(1), value);
            }
            // return the dictionary sorted by the key (date)
            return await ReturnSortDict(dict);
        }

    }
}
