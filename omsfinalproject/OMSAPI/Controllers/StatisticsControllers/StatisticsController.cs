using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using OMSAPI.Dto;
using OMSAPI.Models.Store;
using OMSAPI.Services;
using OMSAPI.Services.ServicesInterfaces;

namespace OMSAPI.Controllers.StatisticsControllers
{
    [ApiController]
    [Route("api/{tenantId}/[controller]")]
    [Authorize(Policy = Roles.Roles.Tenant)]
    public class StatisticsController : ControllerBase
    {
        private ILogger<StatisticsController> _logger;
        private IProductServices _productServices;
        private IOrderServices _orderServices;
        private IStatisticsServices _statisticsServices;

        public StatisticsController(ILogger<StatisticsController> logger, IProductServices productServices, IOrderServices orderServices, IStatisticsServices statisticsServices)
        {
            _logger = logger;
            _productServices = productServices;
            _orderServices = orderServices;
            _statisticsServices = statisticsServices;
        }


        [HttpPost("generalStatsticByDate")]
        public async Task<ActionResult> GetStatistics([FromRoute] string tenantId, [FromBody] DateRangeDto dateRangeDto)
        {

            // Validate that tenant Acess only to his messages.
            if (!AuthServices.ValidateAuthorization(tenantId, HttpContext, User, Roles.Roles.Tenant))
            {
                return Forbid();
            }

            var products = await _productServices.GetAllProducts(tenantId);
            var pendingOrdersAmount = await _orderServices.GetAllOrders(tenantId, OrderStatus.Pending.ToString());
            var readyOrdersAmount = await _orderServices.GetAllOrders(tenantId, OrderStatus.Ready.ToString());
            var deliverdOrdersAmount = await _orderServices.GetAllOrders(tenantId, OrderStatus.Deliverd.ToString());
            var canceledOrdersAmount = await _orderServices.GetAllOrders(tenantId, OrderStatus.Canceled.ToString());

            // validate all the lists are not null
            if (products == null || pendingOrdersAmount == null || readyOrdersAmount == null || deliverdOrdersAmount == null || canceledOrdersAmount == null)
            {
                _logger.LogError($"Failed to get statistics");
                return NotFound();
            }

            // filter by date range
            pendingOrdersAmount = FilterOrdersByDate(pendingOrdersAmount, dateRangeDto);
            readyOrdersAmount = FilterOrdersByDate(readyOrdersAmount, dateRangeDto);
            deliverdOrdersAmount = FilterOrdersByDate(deliverdOrdersAmount, dateRangeDto);

            object statistics = new
            {
                productsAmount = products.Count,
                pendingOrdersAmount = pendingOrdersAmount.Count,
                readyOrdersAmount = readyOrdersAmount.Count,
                deliverdOrdersAmount = deliverdOrdersAmount.Count,
                canceledOrdersAmount = canceledOrdersAmount.Count
            };
            return Ok(statistics);
        }


        [HttpPost("productStatistics/amout")]
        // Write an endpoint that returns the best selling products in a given date range
        public async Task<ActionResult> GetProductStatisticsByAmount([FromRoute] string tenantId, [FromBody] DateRangeDto dateRangeDto)
        {

            // Validate that tenant Acess only to his messages.
            if (!AuthServices.ValidateAuthorization(tenantId, HttpContext, User, Roles.Roles.Tenant))
            {
                return Forbid();
            }

            if (!dateRangeDto.ValidateDateRangoDto())
            {
                return BadRequest($"End date should be bigger than start date".ToJson());
            }

            var products = await _productServices.GetAllProducts(tenantId);
            var orders = await _orderServices.GetAllOrders(tenantId, OrderStatus.Deliverd.ToString());
            // validate all the lists are not null
            if (products == null || orders == null)
            {
                _logger.LogError($"Failed to get statistics");
                return NotFound();
            }

            // filter by date range
            orders = FilterOrdersByDate(orders, dateRangeDto);
            if (orders.Count == 0)
            {
                return NotFound("There are no orders in this period.".ToJson());
            }

            // get all the products that were ordered
            var bestSellingProducts = _statisticsServices.GetOrderedProductsAmount(orders);
            return Ok(bestSellingProducts);
        }

        [HttpPost("productStatistics/price")]
        // Write an endpoint that returns the best selling products in a given date range
        public async Task<ActionResult> GetProductStatisticsByPrice([FromRoute] string tenantId, [FromBody] DateRangeDto dateRangeDto)
        {

            // Validate that tenant Acess only to his messages.
            if (!AuthServices.ValidateAuthorization(tenantId, HttpContext, User, Roles.Roles.Tenant))
            {
                return Forbid();
            }

            if (!dateRangeDto.ValidateDateRangoDto())
            {
                return BadRequest($"End date should be bigger than start date".ToJson());
            }

            var products = await _productServices.GetAllProducts(tenantId);
            var orders = await _orderServices.GetAllOrders(tenantId, OrderStatus.Deliverd.ToString());
            // validate all the lists are not null
            if (products == null || orders == null)
            {
                _logger.LogError($"Failed to get statistics");
                return NotFound();
            }

            // filter by date range
            orders = FilterOrdersByDate(orders, dateRangeDto);

            // get all the products that were ordered
            var bestSellingProducts = _statisticsServices.GetOrderedProductsPrices(orders);
            return Ok(bestSellingProducts);
        }

        [HttpPost("orderStatistics/money")]
        // Write an endpoint that returns the best selling products in a given date range
        public async Task<ActionResult> GetOrderPeriodStatisticsByMoney([FromRoute] string tenantId, [FromBody] DateRangeDto dateRangeDto)
        {
            // Validate that tenant Acess only to his messages.
            if (!AuthServices.ValidateAuthorization(tenantId, HttpContext, User, Roles.Roles.Tenant))
            {
                return Forbid();
            }

            if (!dateRangeDto.ValidateDateRangoDto())
            {
                return BadRequest($"End date should be bigger than start date".ToJson());
            }

            var orders = await _orderServices.GetAllOrders(tenantId, OrderStatus.Deliverd.ToString());

            // validate all the lists are not null
            if (orders == null)
            {
                _logger.LogError($"Failed to get statistics");
                return NotFound();
            }

            // filter by date range
            orders = FilterOrdersByDate(orders, dateRangeDto);
            if (orders.Count == 0)
            {
                return NotFound("There are no orders in this period.".ToJson());
            }

            // get all the products that were ordered
            var orderStatistic = await _statisticsServices.PeriodicOrderMoneyAmout(orders);
            if (orderStatistic != null)
            {
                _logger.LogInformation($"Success to get statistics");
                return Ok(orderStatistic);
            }

            _logger.LogError($"Failed to get statistic");
            return BadRequest();
        }

        [HttpPost("orderStatistics/amount")]
        // Write an endpoint that returns the best selling products in a given date range
        public async Task<ActionResult> GetOrderPeriodStatistics([FromRoute] string tenantId, [FromBody] DateRangeDto dateRangeDto)
        {
            // Validate that tenant Acess only to his messages.
            if (!AuthServices.ValidateAuthorization(tenantId, HttpContext, User, Roles.Roles.Tenant))
            {
                return Forbid();
            }

            if (!dateRangeDto.ValidateDateRangoDto())
            {
                return BadRequest($"End date should be bigger than start date".ToJson());
            }

            var orders = await _orderServices.GetAllOrders(tenantId, OrderStatus.Deliverd.ToString());

            // validate all the lists are not null
            if (orders == null)
            {
                _logger.LogError($"Failed to get statistics");
                return NotFound();
            }

            // filter by date range
            orders = FilterOrdersByDate(orders, dateRangeDto);
            if (orders.Count == 0)
            {
                return NotFound("There are no orders in this period.".ToJson());
            }

            // get all the products that were ordered
            var orderStatistic = await _statisticsServices.PeriodicOrderAmout(orders);
            if (orderStatistic != null)
            {
                _logger.LogInformation($"Success to get statistics");
                return Ok(orderStatistic);
            }

            _logger.LogError($"Failed to get statistic");
            return BadRequest();
        }

        [HttpPost("product/amount/{productId}")]
        public async Task<ActionResult> GetSpesificProductStatisticsAmount([FromRoute] string tenantId, [FromRoute] string productId, [FromBody] DateRangeDto dateRangeDto)
        {
            // Validate that tenant Acess only to his messages.
            if (!AuthServices.ValidateAuthorization(tenantId, HttpContext, User, Roles.Roles.Tenant))
            {
                return Forbid();
            }

            if (!dateRangeDto.ValidateDateRangoDto())
            {
                return BadRequest($"End date should be bigger than start date".ToJson());
            }

            var products = await _productServices.GetProduct(tenantId, productId);
            var orders = await _orderServices.GetAllOrders(tenantId, OrderStatus.Deliverd.ToString());
            // validate all the lists are not null
            if (products == null || orders == null)
            {
                _logger.LogError($"Failed to get statistics");
                return NotFound();
            }
            // filter by date range
            orders = FilterOrdersByDate(orders, dateRangeDto);
            if (orders.Count == 0)
            {
                return NotFound("There are no orders in this period.".ToJson());
            }

            // get all the products that were ordered
            var productStatistic = await _statisticsServices.PeriodicProductAmout(orders, productId);
            if (productStatistic != null)
            {
                _logger.LogInformation($"Success to get statistics");
                return Ok(productStatistic);
            }

            _logger.LogError($"Failed to get statistic");
            return BadRequest();
        }

        [HttpPost("product/money/{productId}")]
        public async Task<ActionResult> GetSpesificProductStatisticsMoney([FromRoute] string tenantId, [FromRoute] string productId, [FromBody] DateRangeDto dateRangeDto)
        {
            // Validate that tenant Acess only to his messages.
            if (!AuthServices.ValidateAuthorization(tenantId, HttpContext, User, Roles.Roles.Tenant))
            {
                return Forbid();
            }

            if (!dateRangeDto.ValidateDateRangoDto())
            {
                return BadRequest($"End date should be bigger than start date".ToJson());
            }

            var products = await _productServices.GetProduct(tenantId, productId);
            var orders = await _orderServices.GetAllOrders(tenantId, OrderStatus.Deliverd.ToString());
            // validate all the lists are not null
            if (products == null || orders == null)
            {
                _logger.LogError($"Failed to get statistics");
                return NotFound();
            }
            // filter by date range
            orders = FilterOrdersByDate(orders, dateRangeDto);
            if (orders.Count == 0)
            {
                return NotFound("There are no orders in this period.".ToJson());
            }

            // get all the products that were ordered
            var productStatistic = await _statisticsServices.PeriodicProductMoneyAmount(orders, productId);
            if (productStatistic != null)
            {
                _logger.LogInformation($"Success to get statistics");
                return Ok(productStatistic);
            }

            _logger.LogError($"Failed to get statistic");
            return BadRequest();
        }

        private List<Order> FilterOrdersByDate(List<Order> list, DateRangeDto dateRangeDto)
        {
            list = list.Where(order => order.OrderDate >= dateRangeDto.Start.ConvertToDateTime()
                            && order.OrderDate <= dateRangeDto.End.ConvertToDateTime()).ToList();
            return list;
        }
    }
}
