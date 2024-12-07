using FakeItEasy;
using OMSAPI.Dto;
using OMSAPI.Models.Store;
using OMSAPI.Services.ServicesInterfaces;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Microsoft.AspNetCore.Mvc;
using OMSAPI.Models.Entities;
using OMSAPI.Controllers.StatisticsControllers;

namespace OMSAPITesting.Controllers
{
    public class StatisticsControllerTests
    {
        private readonly ILogger<StatisticsController> _logger;
        private readonly IProductServices _productServices;
        private readonly IOrderServices _orderServices;
        private readonly IStatisticsServices _statisticsServices;
        private readonly StatisticsController _controller;

        public StatisticsControllerTests()
        {
            _logger = A.Fake<ILogger<StatisticsController>>();
            _productServices = A.Fake<IProductServices>();
            _orderServices = A.Fake<IOrderServices>();
            _statisticsServices = A.Fake<IStatisticsServices>();
            _controller = new StatisticsController(_logger, _productServices, _orderServices, _statisticsServices);

        }

        /* GetStatistics Tests Function */
        [Fact]
        public async void GetProductStatistics_Returns_ReturnForbid()
        {
            // Arrange
            // Create a default http context with a tenant
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant1 = A.Fake<Tenant>();
            tenant1.Id = tenantId;
            var tenant2 = A.Fake<Tenant>();
            tenant2.Id = ObjectId.GenerateNewId().ToString();

            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant2);
            _controller.ControllerContext.HttpContext = httpContext;

            var dateRangeDto = new DateRangeDto();
            A.CallTo(() => _productServices.GetAllProducts(tenantId)).Returns((List<Product>?)null);
            // Act
            var response = await _controller.GetStatistics(tenantId, dateRangeDto);
            // Assert
            Assert.IsType<ForbidResult>(response);

        }

        [Fact]
        public async void GetProductStatistics_Returns_ReturnOk()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var dateRangeDto = new DateRangeDto();

            // Create a default http context with a tenant
            var tenant = A.Fake<Tenant>();
            tenant.Id = tenantId;
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            var products = A.Fake<List<Product>>();
            var orders = A.Fake<List<Order>>();
            var dict = A.Fake<Dictionary<string, int>>();



            A.CallTo(() => _productServices.GetAllProducts(tenantId)).Returns(products);
            A.CallTo(() => _orderServices.GetAllOrders(tenantId, OrderStatus.Deliverd.ToString())).Returns(orders);
            A.CallTo(() => _statisticsServices.GetOrderedProductsAmount(orders)).Returns(dict);

            // Act
            var response = await _controller.GetStatistics(tenantId, dateRangeDto);

            // Assert
            Assert.IsType<OkObjectResult>(response);
            
        }

        [Fact]
        public async void GetProductStatistics_OrdersReturnsNull_NotFound()
        {
            // Arrange
            // Create a default http context with a tenant
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = tenantId;
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            var dateRangeDto = new DateRangeDto();
            var products = A.Fake<List<Product>>();

            A.CallTo(() => _productServices.GetAllProducts(tenantId)).Returns((List<Product>?)products);
            A.CallTo(() => _orderServices.GetAllOrders(tenantId, OrderStatus.Deliverd.ToString())).Returns((List<Order>?)null);

            // Act
            var response = await _controller.GetStatistics(tenantId, dateRangeDto);

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async void GetProductStatistics_ProductsReturnsNull_NotFound()
        {
            // Arrange
            // Create a default http context with a tenant
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = tenantId;
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            var dateRangeDto = new DateRangeDto();
            var orders = A.Fake<List<Order>>();

            A.CallTo(() => _productServices.GetAllProducts(tenantId)).Returns((List<Product>?)null);
            A.CallTo(() => _orderServices.GetAllOrders(tenantId, OrderStatus.Deliverd.ToString())).Returns((List<Order>?)orders);

            // Act
            var response = await _controller.GetStatistics(tenantId, dateRangeDto);

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }


        /* GetProductStatisticsByAmount Tests Function */
        [Fact]
        public async void GetProductStatisticsByAmount_Returns_ReturnForbid()
        {
            // Arrange
            // Create a default http context with a tenant
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant1 = A.Fake<Tenant>();
            tenant1.Id = tenantId;
            var tenant2 = A.Fake<Tenant>();
            tenant2.Id = ObjectId.GenerateNewId().ToString();

            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant2);
            _controller.ControllerContext.HttpContext = httpContext;

            var dateRangeDto = new DateRangeDto();
            A.CallTo(() => _productServices.GetAllProducts(tenantId)).Returns((List<Product>?)null);
            // Act
            var response = await _controller.GetProductStatisticsByAmount(tenantId, dateRangeDto);
            // Assert
            Assert.IsType<ForbidResult>(response);

        }

        [Fact]
        public async void GetProductStatisticsByAmount_OrdersReturnsNull_NotFound()
        {
            // Arrange
            // Create a default http context with a tenant
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = tenantId;
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            DateDto start = new DateDto(2023, 1, 1);
            DateDto end = new DateDto(2023, 7, 1);
            var dateRangeDto = new DateRangeDto(start, end);
            var orders = A.Fake<List<Order>>();

            A.CallTo(() => _productServices.GetAllProducts(tenantId)).Returns((List<Product>?)null);
            A.CallTo(() => _orderServices.GetAllOrders(tenantId, OrderStatus.Deliverd.ToString())).Returns((List<Order>?)orders);

            // Act
            var response = await _controller.GetProductStatisticsByAmount(tenantId, dateRangeDto);

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async void GetProductStatisticsByAmount_ProductsReturnsNull_NotFound()
        {
            // Arrange
            // Create a default http context with a tenant
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = tenantId;
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            DateDto start = new DateDto(2023, 1, 1);
            DateDto end = new DateDto(2023, 7, 1);
            var dateRangeDto = new DateRangeDto(start, end);
            var products = A.Fake<List<Product>>();

            A.CallTo(() => _productServices.GetAllProducts(tenantId)).Returns((List<Product>?)products);
            A.CallTo(() => _orderServices.GetAllOrders(tenantId, OrderStatus.Deliverd.ToString())).Returns((List<Order>?)null);

            // Act
            var response = await _controller.GetProductStatisticsByAmount(tenantId, dateRangeDto);

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async void GetProductStatisticsByAmount_Returns_ReturnOk()
        {
            // Arrange
            // Create a default http context with a tenant
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = tenantId;
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            DateDto start = new DateDto(2023, 1, 1);
            DateDto end = new DateDto(2023, 7, 1);
            var dateRangeDto = new DateRangeDto(start, end);

            var products = A.Fake<List<Product>>();
            var order = A.Fake<Order>();
            var orders = A.Fake<List<Order>>();
            orders.Add(order);
            var dict = A.Fake<Dictionary<string, int>>();


            A.CallTo(() => _productServices.GetAllProducts(tenantId)).Returns(products);
            A.CallTo(() => _orderServices.GetAllOrders(tenantId, OrderStatus.Deliverd.ToString())).Returns(orders);

            // Act
            var response = await _controller.GetProductStatisticsByAmount(tenantId, dateRangeDto);

            // Assert
            Assert.IsType<OkObjectResult>(response);
            Assert.Equal(200, ((OkObjectResult)response).StatusCode);
        }


        /* GetProductStatisticsByPrice Tests Function */
        [Fact]
        public async void GetProductStatisticsByPrice_Returns_ReturnForbid()
        {
            // Arrange
            // Create a default http context with a tenant
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant1 = A.Fake<Tenant>();
            tenant1.Id = tenantId;
            var tenant2 = A.Fake<Tenant>();
            tenant2.Id = ObjectId.GenerateNewId().ToString();

            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant2);
            _controller.ControllerContext.HttpContext = httpContext;

            DateDto start = new DateDto(2023, 1, 1);
            DateDto end = new DateDto(2023, 7, 1);
            var dateRangeDto = new DateRangeDto(start, end);

            A.CallTo(() => _productServices.GetAllProducts(tenantId)).Returns((List<Product>?)null);
            // Act
            var response = await _controller.GetProductStatisticsByPrice(tenantId, dateRangeDto);
            // Assert
            Assert.IsType<ForbidResult>(response);
        }

        [Fact]
        public async void GetProductStatisticsByPrice_OrdersReturnsNull_NotFound()
        {
            // Arrange
            // Create a default http context with a tenant
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = tenantId;
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            DateDto start = new DateDto(2023, 1, 1);
            DateDto end = new DateDto(2023, 7, 1);
            var dateRangeDto = new DateRangeDto(start, end);
            var orders = A.Fake<List<Order>>();

            A.CallTo(() => _productServices.GetAllProducts(tenantId)).Returns((List<Product>?)null);
            A.CallTo(() => _orderServices.GetAllOrders(tenantId, OrderStatus.Deliverd.ToString())).Returns((List<Order>?)orders);

            // Act
            var response = await _controller.GetProductStatisticsByPrice(tenantId, dateRangeDto);

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async void GetProductStatisticsByPrice_ProductsReturnsNull_NotFound()
        {
            // Arrange
            // Create a default http context with a tenant
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = tenantId;
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            DateDto start = new DateDto(2023, 1, 1);
            DateDto end = new DateDto(2023, 7, 1);
            var dateRangeDto = new DateRangeDto(start, end);
            var products = A.Fake<List<Product>>();

            A.CallTo(() => _productServices.GetAllProducts(tenantId)).Returns((List<Product>?)products);
            A.CallTo(() => _orderServices.GetAllOrders(tenantId, OrderStatus.Deliverd.ToString())).Returns((List<Order>?)null);

            // Act
            var response = await _controller.GetProductStatisticsByPrice(tenantId, dateRangeDto);

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async void GetProductStatisticsByPrice_Returns_ReturnOk()
        {
            // Arrange
            // Create a default http context with a tenant
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = tenantId;
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            DateDto start = new DateDto(2023, 1, 1);
            DateDto end = new DateDto(2023, 7, 1);
            var dateRangeDto = new DateRangeDto(start, end);

            var products = A.Fake<List<Product>>();
            var order = A.Fake<Order>();
            var orders = A.Fake<List<Order>>();
            orders.Add(order);
            var dict = A.Fake<Dictionary<string, decimal>>();


            A.CallTo(() => _productServices.GetAllProducts(tenantId)).Returns(products);
            A.CallTo(() => _orderServices.GetAllOrders(tenantId, OrderStatus.Deliverd.ToString())).Returns(orders);

            // Act
            var response = await _controller.GetProductStatisticsByPrice(tenantId, dateRangeDto);

            // Assert
            Assert.IsType<OkObjectResult>(response);
        }

        /* GetOrderPeriodStatisticsByMoney Tests Functions */
        [Fact]
        public async void GetOrderPeriodStatisticsByMoney_Returns_ReturnForbid()
        {
            // Arrange
            // Create a default http context with a tenant
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant1 = A.Fake<Tenant>();
            tenant1.Id = tenantId;
            var tenant2 = A.Fake<Tenant>();
            tenant2.Id = ObjectId.GenerateNewId().ToString();

            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant2);
            _controller.ControllerContext.HttpContext = httpContext;

            var dateRangeDto = new DateRangeDto();
            A.CallTo(() => _productServices.GetAllProducts(tenantId)).Returns((List<Product>?)null);
            // Act
            var response = await _controller.GetOrderPeriodStatisticsByMoney(tenantId, dateRangeDto);
            // Assert
            Assert.IsType<ForbidResult>(response);
        }

        [Fact]
        public async void GetOrderPeriodStatisticsByMoney_OrdersReturnsNull_NotFound()
        {
            // Arrange
            // Create a default http context with a tenant
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = tenantId;
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            DateDto start = new DateDto(2023, 1, 1);
            DateDto end = new DateDto(2023, 7, 1);
            var dateRangeDto = new DateRangeDto(start, end);

            A.CallTo(() => _orderServices.GetAllOrders(tenantId, OrderStatus.Deliverd.ToString())).Returns((List<Order>?)null);

            // Act
            var response = await _controller.GetOrderPeriodStatisticsByMoney(tenantId, dateRangeDto);

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async void GetOrderPeriodStatisticsByMoney_Returns_ReturnOk()
        {
            // Arrange
            // Create a default http context with a tenant
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = tenantId;
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            DateDto start = new DateDto(2023, 1, 1);
            DateDto end = new DateDto(2023, 7, 1);
            var dateRangeDto = new DateRangeDto(start, end);

            var order = A.Fake<Order>();
            var orders = A.Fake<List<Order>>();
            orders.Add(order);
            var dict = A.Fake<Dictionary<DateOnly, decimal>>();

            A.CallTo(() => _orderServices.GetAllOrders(tenantId, OrderStatus.Deliverd.ToString())).Returns(orders);
            A.CallTo(() => _statisticsServices.PeriodicOrderMoneyAmout(orders)).Returns(dict);

            // Act
            var response = await _controller.GetOrderPeriodStatisticsByMoney(tenantId, dateRangeDto);

            // Assert
            Assert.IsType<OkObjectResult>(response);
        }

        /* GetOrderPeriodStatistics Tests Functions */
        [Fact]
        public async void GetOrderPeriodStatistics_Returns_ReturnForbid()
        {
            // Arrange
            // Create a default http context with a tenant
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant1 = A.Fake<Tenant>();
            tenant1.Id = tenantId;
            var tenant2 = A.Fake<Tenant>();
            tenant2.Id = ObjectId.GenerateNewId().ToString();

            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant2);
            _controller.ControllerContext.HttpContext = httpContext;

            DateDto start = new DateDto(2023, 1, 1);
            DateDto end = new DateDto(2023, 7, 1);
            var dateRangeDto = new DateRangeDto(start, end);
            A.CallTo(() => _productServices.GetAllProducts(tenantId)).Returns((List<Product>?)null);
            // Act
            var response = await _controller.GetOrderPeriodStatistics(tenantId, dateRangeDto);
            // Assert
            Assert.IsType<ForbidResult>(response);
        }

        [Fact]
        public async void GetOrderPeriodStatistics_OrdersReturnsNull_NotFound()
        {
            // Arrange
            // Create a default http context with a tenant
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = tenantId;
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            DateDto start = new DateDto(2023, 1, 1);
            DateDto end = new DateDto(2023, 7, 1);
            var dateRangeDto = new DateRangeDto(start, end);

            A.CallTo(() => _orderServices.GetAllOrders(tenantId, OrderStatus.Deliverd.ToString())).Returns((List<Order>?)null);

            // Act
            var response = await _controller.GetOrderPeriodStatistics(tenantId, dateRangeDto);

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async void GetOrderPeriodStatistics_Returns_ReturnOk()
        {
            // Arrange
            // Create a default http context with a tenant
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = tenantId;
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            DateDto start = new DateDto(2023, 1, 1);
            DateDto end = new DateDto(2023, 7, 1);
            var dateRangeDto = new DateRangeDto(start, end);

            var order = A.Fake<Order>();
            var orders = A.Fake<List<Order>>();
            orders.Add(order);
            var dict = A.Fake<Dictionary<DateOnly, int>>();

            A.CallTo(() => _orderServices.GetAllOrders(tenantId, OrderStatus.Deliverd.ToString())).Returns(orders);
            A.CallTo(() => _statisticsServices.PeriodicOrderAmout(orders)).Returns(dict);
            
            // Act
            var response = await _controller.GetOrderPeriodStatistics(tenantId, dateRangeDto);

            // Assert
            Assert.IsType<OkObjectResult>(response);
        }

        /* GetSpesificProductStatisticsAmount Tests Functions */
        [Fact]
        public async void GetSpesificProductStatisticsAmount_Returns_ReturnForbid()
        {
            // Arrange
            // Create a default http context with a tenant
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant1 = A.Fake<Tenant>();
            tenant1.Id = tenantId;
            var tenant2 = A.Fake<Tenant>();
            tenant2.Id = ObjectId.GenerateNewId().ToString();
            var productId = ObjectId.GenerateNewId().ToString();

            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant2);
            _controller.ControllerContext.HttpContext = httpContext;

            var dateRangeDto = new DateRangeDto();
            A.CallTo(() => _productServices.GetAllProducts(tenantId)).Returns((List<Product>?)null);
            // Act
            var response = await _controller.GetSpesificProductStatisticsAmount(tenantId, productId, dateRangeDto);
            // Assert
            Assert.IsType<ForbidResult>(response);
        }

        [Fact]
        public async void GetSpesificProductStatisticsAmount_OrdersReturnsNull_NotFound()
        {
            // Arrange
            // Create a default http context with a tenant
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = tenantId;
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            DateDto start = new DateDto(2023,1,1);
            DateDto end = new DateDto(2023, 7, 1);
            var dateRangeDto = new DateRangeDto(start,end);
            var product = A.Fake<Product>();
            product.Id = ObjectId.GenerateNewId().ToString();

            A.CallTo(() => _orderServices.GetAllOrders(tenantId, OrderStatus.Deliverd.ToString())).Returns((List<Order>?)null);

            // Act
            var response = await _controller.GetSpesificProductStatisticsAmount(tenantId, product.Id, dateRangeDto);

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async void GetSpesificProductStatisticsAmount_GetProductReturnsNull_NotFound()
        {
            // Arrange
            // Create a default http context with a tenant
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = tenantId;
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            DateDto start = new DateDto(2023, 1, 1);
            DateDto end = new DateDto(2023, 7, 1);
            var dateRangeDto = new DateRangeDto(start, end);
            var product = A.Fake<Product>();
            product.Id = ObjectId.GenerateNewId().ToString();

            A.CallTo(() => _orderServices.GetAllOrders(tenantId, OrderStatus.Deliverd.ToString())).Returns((List<Order>?)null);

            // Act
            var response = await _controller.GetSpesificProductStatisticsAmount(tenantId, product.Id, dateRangeDto);

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async void GetSpesificProductStatisticsAmount_Returns_ReturnOk()
        {
            // Arrange
            // Create a default http context with a tenant
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = tenantId;
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            DateDto start = new DateDto(2023, 1, 1);
            DateDto end = new DateDto(2023, 7, 1);
            var dateRangeDto = new DateRangeDto(start, end);
            var product = A.Fake<Product>();
            var productId = ObjectId.GenerateNewId().ToString();
            product.Id = productId;

            var order = A.Fake<Order>();
            var orders = A.Fake<List<Order>>();
            orders.Add(order);
            var dict = A.Fake<Dictionary<string, int>>();

            A.CallTo(() => _orderServices.GetAllOrders(tenantId, OrderStatus.Deliverd.ToString())).Returns(orders);
            A.CallTo(() => _productServices.GetProduct(tenantId, productId)).Returns(product);
            A.CallTo(() => _statisticsServices.GetOrderedProductsAmount(orders)).Returns(dict);

            // Act
            var response = await _controller.GetSpesificProductStatisticsAmount(tenantId, productId, dateRangeDto);

            // Assert
            Assert.IsType<OkObjectResult>(response);
            Assert.Equal(dict, ((OkObjectResult)response).Value);
        }

        /* GetSpesificProductStatisticsMoney Tests Functions */
        [Fact]
        public async void GetSpesificProductStatisticsMoney_Returns_ReturnForbid()
        {
            // Arrange
            // Create a default http context with a tenant
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant1 = A.Fake<Tenant>();
            tenant1.Id = tenantId;
            var tenant2 = A.Fake<Tenant>();
            tenant2.Id = ObjectId.GenerateNewId().ToString();
            var productId = ObjectId.GenerateNewId().ToString();

            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant2);
            _controller.ControllerContext.HttpContext = httpContext;

            DateDto start = new DateDto(2023, 1, 1);
            DateDto end = new DateDto(2023, 7, 1);
            var dateRangeDto = new DateRangeDto(start, end);
            A.CallTo(() => _productServices.GetAllProducts(tenantId)).Returns((List<Product>?)null);
            // Act
            var response = await _controller.GetSpesificProductStatisticsMoney(tenantId, productId, dateRangeDto);
            // Assert
            Assert.IsType<ForbidResult>(response);
        }

        [Fact]
        public async void GetSpesificProductStatisticsMoney_OrdersReturnsNull_NotFound()
        {
            // Arrange
            // Create a default http context with a tenant
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = tenantId;
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            DateDto start = new DateDto(2023, 1, 1);
            DateDto end = new DateDto(2023, 7, 1);
            var dateRangeDto = new DateRangeDto(start, end);
            var product = A.Fake<Product>();
            product.Id = ObjectId.GenerateNewId().ToString();

            A.CallTo(() => _orderServices.GetAllOrders(tenantId, OrderStatus.Deliverd.ToString())).Returns((List<Order>?)null);

            // Act
            var response = await _controller.GetSpesificProductStatisticsMoney(tenantId, product.Id, dateRangeDto);

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async void GetSpesificProductStatisticsMoney_GetProductReturnsNull_NotFound()
        {
            // Arrange
            // Create a default http context with a tenant
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = tenantId;
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            DateDto start = new DateDto(2023, 1, 1);
            DateDto end = new DateDto(2023, 7, 1);
            var dateRangeDto = new DateRangeDto(start, end);
            var product = A.Fake<Product>();
            product.Id = ObjectId.GenerateNewId().ToString();

            A.CallTo(() => _orderServices.GetAllOrders(tenantId, OrderStatus.Deliverd.ToString())).Returns((List<Order>?)null);

            // Act
            var response = await _controller.GetSpesificProductStatisticsAmount(tenantId, product.Id, dateRangeDto);

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async void GetSpesificProductStatisticsMoney_Returns_ReturnOk()
        {
            // Arrange
            // Create a default http context with a tenant
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = tenantId;
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            DateDto start = new DateDto(2023, 1, 1);
            DateDto end = new DateDto(2023, 7, 1);
            var dateRangeDto = new DateRangeDto(start, end);
            var product = A.Fake<Product>();
            var productId = ObjectId.GenerateNewId().ToString();
            product.Id = productId;

            var order = A.Fake<Order>();
            var orders = A.Fake<List<Order>>();
            orders.Add(order);
            var dict = A.Fake<Dictionary<string, decimal>>();

            A.CallTo(() => _orderServices.GetAllOrders(tenantId, OrderStatus.Deliverd.ToString())).Returns(orders);
            A.CallTo(() => _productServices.GetProduct(tenantId, productId)).Returns(product);
            A.CallTo(() => _statisticsServices.GetOrderedProductsPrices(orders)).Returns(dict);

            // Act
            var response = await _controller.GetSpesificProductStatisticsMoney(tenantId, productId, dateRangeDto);

            // Assert
            Assert.IsType<OkObjectResult>(response);
            Assert.Equal(dict, ((OkObjectResult)response).Value);

        }
    }
}
