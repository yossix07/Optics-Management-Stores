using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using OMSAPI.Controllers.StoreControllers;
using OMSAPI.Models.Entities;
using OMSAPI.Models.Store;
using OMSAPI.Services.ServicesInterfaces;

namespace OMSAPITesting.Controllers
{
    public class ProductControllerTest
    {
        private readonly IProductServices _productServices;
        private readonly ILogger<ProductController> _logger;
        private readonly ProductController _controller;
        public ProductControllerTest()
        {
            _productServices = A.Fake<IProductServices>();
            _logger = A.Fake<ILogger<ProductController>>();
            _controller = new ProductController(_productServices, _logger);
        }

        /* GetAllProductsAsync Test Functions */
        [Fact]
        public async Task ProductController_GetAllProductsAsync_ReturnsOkObjectResult()
        {
            // Arrange
            var tenant = A.Fake<Tenant>();
            tenant.Id = ObjectId.GenerateNewId().ToString();

            // create http context
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            var products = A.Fake<List<Product>>();
            A.CallTo(() => _productServices.GetAllProducts(tenant.Id)).Returns(products);
            // Act
            var result = await _controller.GetAllProductsAsync(tenant.Id);
            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task ProductController_GetAllProductsAsync_ReturnsForbid()
        {
            // Arrange
            var tenant1 = A.Fake<Tenant>();
            tenant1.Id = ObjectId.GenerateNewId().ToString();
            var tenant2 = A.Fake<Tenant>();
            tenant2.Id = ObjectId.GenerateNewId().ToString();

            // create http context
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant2);
            _controller.ControllerContext.HttpContext = httpContext;

            var products = A.Fake<List<Product>>();
            A.CallTo(() => _productServices.GetAllProducts(tenant1.Id)).Returns(products);
            // Act
            var result = await _controller.GetAllProductsAsync(tenant1.Id);
            // Assert
            Assert.IsType<ForbidResult>(result.Result);
        }

        [Fact]
        public async Task ProductController_GetAllProductsAsync_ReturnsBadRequest()
        {
            // Arrange
            var tenant = A.Fake<Tenant>();
            tenant.Id = ObjectId.GenerateNewId().ToString();

            // create http context
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            var products = A.Fake<List<Product>>();
            A.CallTo(() => _productServices.GetAllProducts(tenant.Id)).Returns<List<Product>?>(null);
            // Act?
            var result = await _controller.GetAllProductsAsync(tenant.Id);
            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        /* Get Test Functions */
        [Fact]
        public async Task ProductController_Get_ReturnsForbid()
        {
            // Arrange
            var tenant1 = A.Fake<Tenant>();
            tenant1.Id = ObjectId.GenerateNewId().ToString();
            var tenant2 = A.Fake<Tenant>();
            tenant2.Id = ObjectId.GenerateNewId().ToString();

            var product = A.Dummy<Product>();
            product.Id = ObjectId.GenerateNewId().ToString();

            // create http context
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant2);
            _controller.ControllerContext.HttpContext = httpContext;

            A.CallTo(() => _productServices.GetProduct(tenant1.Id, product.Id)).Returns(product);
            // Act
            var result = await _controller.Get(tenant1.Id, product.Id);
            // Assert
            Assert.IsType<ForbidResult>(result.Result);
        }

        [Fact]
        public async Task ProductController_Get_ReturnsNotFound()
        {
            // Arrange
            var tenant = A.Fake<Tenant>();
            tenant.Id = ObjectId.GenerateNewId().ToString();

            // create http context
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            var product = A.Dummy<Product>();
            product.Id = ObjectId.GenerateNewId().ToString();

            A.CallTo(() => _productServices.GetProduct(tenant.Id, product.Id)).Returns<Product?>(null);
            // Act
            var result = await _controller.Get(tenant.Id, product.Id);
            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task ProductController_Get_ReturnsOk()
        {
            // Arrange
            var tenant = A.Fake<Tenant>();
            tenant.Id = ObjectId.GenerateNewId().ToString();

            // create http context
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            var product = A.Dummy<Product>();
            product.Id = ObjectId.GenerateNewId().ToString();

            A.CallTo(() => _productServices.GetProduct(tenant.Id, product.Id)).Returns<Product?>(product);
            // Act
            var result = await _controller.Get(tenant.Id, product.Id);
            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        /* CreateProductAsync Test Functions */
        [Fact]
        public async Task ProductController_CreateProductAsync_ReturnsForbid()
        {
            // Arrange
            var tenant1 = A.Fake<Tenant>();
            tenant1.Id = ObjectId.GenerateNewId().ToString();
            var tenant2 = A.Fake<Tenant>();
            tenant2.Id = ObjectId.GenerateNewId().ToString();

            var product = A.Dummy<Product>();
            product.Id = ObjectId.GenerateNewId().ToString();

            // create http context
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant2);
            _controller.ControllerContext.HttpContext = httpContext;

            A.CallTo(() => _productServices.CreateProduct(tenant1.Id, product)).Returns(product);
            // Act
            var result = await _controller.CreateProductAsync(tenant1.Id, product);
            // Assert
            Assert.IsType<ForbidResult>(result.Result);
        }

        [Fact]
        public async Task ProductController_CreateProductAsync_ReturnsBadRequest()
        {
            // Arrange
            var tenant = A.Fake<Tenant>();
            tenant.Id = ObjectId.GenerateNewId().ToString();

            // create http context
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            var product = A.Dummy<Product>();
            product.Id = ObjectId.GenerateNewId().ToString();

            A.CallTo(() => _productServices.CreateProduct(tenant.Id, product)).Returns<Product?>(null);
            // Act
            var result = await _controller.CreateProductAsync(tenant.Id, product);
            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task ProductController_CreateProductAsync_ReturnsOk()
        {
            // Arrange
            var tenant = A.Fake<Tenant>();
            tenant.Id = ObjectId.GenerateNewId().ToString();

            // create http context
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            var product = A.Dummy<Product>();
            product.Id = ObjectId.GenerateNewId().ToString();

            A.CallTo(() => _productServices.CreateProduct(tenant.Id, product)).Returns<Product?>(product);
            // Act
            var result = await _controller.CreateProductAsync(tenant.Id, product);
            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        /* PutProductAsync Test Functions */
        [Fact]
        public async Task ProductController_PutProductAsync_ReturnsForbid()
        {
            // Arrange
            var tenant1 = A.Fake<Tenant>();
            tenant1.Id = ObjectId.GenerateNewId().ToString();
            var tenant2 = A.Fake<Tenant>();
            tenant2.Id = ObjectId.GenerateNewId().ToString();

            var product = A.Dummy<Product>();
            product.Id = ObjectId.GenerateNewId().ToString();

            // create http context
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant2);
            _controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = await _controller.PutProductAsync(tenant1.Id,product.Id, product);
            // Assert
            Assert.IsType<ForbidResult>(result.Result);
        }

        [Fact]
        public async Task ProductController_PutProductAsync_GetProductFailed_ReturnsBadRequest()
        {
            // Arrange
            var tenant = A.Fake<Tenant>();
            tenant.Id = ObjectId.GenerateNewId().ToString();

            // create http context
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            var product = A.Dummy<Product>();
            product.Id = ObjectId.GenerateNewId().ToString();

            A.CallTo(() => _productServices.GetProduct(tenant.Id, product.Id)).Returns<Product?>(null);
            // Act
            var result = await _controller.PutProductAsync(tenant.Id, product.Id, product);
            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task ProductController_PutProductAsync_UpdateProductFailed_ReturnsBadRequest()
        {
            // Arrange
            var tenant = A.Fake<Tenant>();
            tenant.Id = ObjectId.GenerateNewId().ToString();

            // create http context
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            var product = A.Dummy<Product>();
            product.Id = ObjectId.GenerateNewId().ToString();

            A.CallTo(() => _productServices.GetProduct(tenant.Id, product.Id)).Returns<Product?>(product);
            A.CallTo(() => _productServices.UpdateProduct(tenant.Id,product.Id, product)).Returns<Product?>(null);
            
            // Act
            var result = await _controller.PutProductAsync(tenant.Id, product.Id, product);
            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task ProductController_PutProductAsync_ReturnOk()
        {
            // Arrange
            var tenant = A.Fake<Tenant>();
            tenant.Id = ObjectId.GenerateNewId().ToString();

            // create http context
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            var product = A.Dummy<Product>();
            product.Id = ObjectId.GenerateNewId().ToString();

            A.CallTo(() => _productServices.GetProduct(tenant.Id, product.Id)).Returns<Product?>(product);
            A.CallTo(() => _productServices.UpdateProduct(tenant.Id, product.Id, product)).Returns<Product?>(product);

            // Act
            var result = await _controller.PutProductAsync(tenant.Id, product.Id, product);
            // Assert
            Assert.IsType<OkResult>(result.Result);
        }


        /* DeleteProductAsync Test Functions */
        [Fact]
        public async Task ProductController_DeleteProductAsync_ReturnsForbid()
        {
            // Arrange
            var tenant1 = A.Fake<Tenant>();
            tenant1.Id = ObjectId.GenerateNewId().ToString();
            var tenant2 = A.Fake<Tenant>();
            tenant2.Id = ObjectId.GenerateNewId().ToString();

            var product = A.Dummy<Product>();
            product.Id = ObjectId.GenerateNewId().ToString();

            // create http context
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant2);
            _controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = await _controller.DeleteProductAsync(tenant1.Id, product.Id);
            // Assert
            Assert.IsType<ForbidResult>(result.Result);
        }

        [Fact]
        public async Task ProductController_DeleteProductAsync_GetProductFailed_ReturnsBadRequest()
        {
            // Arrange
            var tenant = A.Fake<Tenant>();
            tenant.Id = ObjectId.GenerateNewId().ToString();

            // create http context
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            var product = A.Dummy<Product>();
            product.Id = ObjectId.GenerateNewId().ToString();

            A.CallTo(() => _productServices.GetProduct(tenant.Id, product.Id)).Returns<Product?>(null);
            // Act
            var result = await _controller.DeleteProductAsync(tenant.Id, product.Id);
            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task ProductController_DeleteProductAsync_DeleteProductFailed_ReturnsBadRequest()
        {
            // Arrange
            var tenant = A.Fake<Tenant>();
            tenant.Id = ObjectId.GenerateNewId().ToString();

            // create http context
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            var product = A.Dummy<Product>();
            product.Id = ObjectId.GenerateNewId().ToString();

            A.CallTo(() => _productServices.GetProduct(tenant.Id, product.Id)).Returns<Product?>(product);
            A.CallTo(() => _productServices.DeleteProduct(tenant.Id, product)).Returns<Product?>(null);

            // Act
            var result = await _controller.DeleteProductAsync(tenant.Id, product.Id);
            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task ProductController_DeleteProductAsync_ReturnOk()
        {
            // Arrange
            var tenant = A.Fake<Tenant>();
            tenant.Id = ObjectId.GenerateNewId().ToString();

            // create http context
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            var product = A.Dummy<Product>();
            product.Id = ObjectId.GenerateNewId().ToString();

            A.CallTo(() => _productServices.GetProduct(tenant.Id, product.Id)).Returns<Product?>(product);
            A.CallTo(() => _productServices.DeleteProduct(tenant.Id, product)).Returns<Product?>(product);

            // Act
            var result = await _controller.DeleteProductAsync(tenant.Id, product.Id);
            // Assert
            Assert.IsType<OkResult>(result.Result);
        }
    }
}
