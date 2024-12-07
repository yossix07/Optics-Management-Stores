using OMSAPI.Models.Entities;
using OMSAPI.Services.ServicesInterfaces;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using FluentAssertions;
using OMSAPI.Dto.EntitiesDto;
using OMSAPI.Controllers.EntitiesControllers;

namespace OMSAPITesting.Controllers
{
    public class TenantControllerTest
    {
        private readonly ITenantServices _tenantService;
        private readonly ILogger<TenantController> _logger;
        private readonly TenantController _controller;
        public TenantControllerTest()
        {
            _tenantService = A.Fake<ITenantServices>();
            _logger = A.Fake<ILogger<TenantController>>();
            _controller = new TenantController(_tenantService, _logger);
        }

        /* GetAll Test Functions */
        [Fact]
        public async Task TenantController_GetAll_ReturnsOk()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = tenantId;
            var list = new List<Tenant> { tenant};
            A.CallTo(() => _tenantService.GetAll()).Returns(list);

            // Create a new ClaimsIdentity for the user
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);

            // Set the controller HttpContext to the new HttpContext
            _controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = await _controller.GetAll();

            // Assert
            result.Result.Should().NotBeNull();
            result.Result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async Task TenantController_GetAll_ReturnsBadRequest()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = tenantId;
            var list = new List<Tenant> { tenant };
            A.CallTo(() => _tenantService.GetAll()).Returns<List<Tenant>?>(null);

            // Create a new ClaimsIdentity for the user
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);

            // Set the controller HttpContext to the new HttpContext
            _controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = await _controller.GetAll();

            // Assert
            result.Result.Should().NotBeNull();
            result.Result.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        /* Get Test Functions */
        [Fact]
        public async Task TenantController_Get_ReturnOk()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = tenantId;
            A.CallTo(() => _tenantService.Get(tenantId)).Returns(tenant);

            // Create a new ClaimsIdentity for the user
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);

            // Set the controller HttpContext to the new HttpContext
            _controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = await _controller.Get(tenantId);

            // Assert
            result.Result.Should().NotBeNull();
            result.Result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async Task TenantController_Get_ReturnForbid()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant1 = A.Fake<Tenant>();
            tenant1.Id = tenantId;
            var tenant2 = A.Fake<Tenant>();
            tenant2.Id = "123";
            A.CallTo(() => _tenantService.Get(tenant1.Id)).Returns(tenant1);

            // Create a new ClaimsIdentity for the user
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant2);

            // Set the controller HttpContext to the new HttpContext
            _controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = await _controller.Get(tenant1.Id);

            // Assert
            result.Result.Should().NotBeNull();
            result.Result.Should().BeOfType(typeof(ForbidResult));
        }

        [Fact]
        public async Task TenantController_Get_ReturnNotFound()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = tenantId;
            A.CallTo(() => _tenantService.Get(tenantId)).Returns<Tenant?>(null);

            // Create a new ClaimsIdentity for the user
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);

            // Set the controller HttpContext to the new HttpContext
            _controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = await _controller.Get(tenantId);

            // Assert
            result.Result.Should().NotBeNull();
            result.Result.Should().BeOfType(typeof(NotFoundObjectResult));
        }



        /* Put Test Functions */
        [Fact]
        public async Task TenantCollector_Put_ReturnOk()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = tenantId;
            tenant.Name = "oldTenantName";
            var newTenantDto = A.Fake<UpdateTenantDto>();
            newTenantDto.Email = "test@gmail.com";
            newTenantDto.Name = "newTenantTestName";
            newTenantDto.Address = "newTenantTestAddress";
            newTenantDto.PhoneNumber = "054-21212121";

            A.CallTo(() => _tenantService.Get(tenantId)).Returns(tenant);
            A.CallTo(() => _tenantService.Update(tenantId, newTenantDto)).Returns(tenant);

            // Create a new ClaimsIdentity for the user
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);

            // Set the controller HttpContext to the new HttpContext
            _controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = await _controller.Put(tenantId, newTenantDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkResult));
        }

        [Fact]
        public async Task TenantController_Put_ReturnForbid()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant1 = A.Fake<Tenant>();
            tenant1.Id = tenantId;
            var tenant2 = A.Fake<Tenant>();
            tenant2.Id = "123";
            A.CallTo(() => _tenantService.Get(tenant1.Id)).Returns(tenant1);

            // Create a new ClaimsIdentity for the user
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant2);

            // Set the controller HttpContext to the new HttpContext
            _controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = await _controller.Put(tenant1.Id, A.Fake<UpdateTenantDto>()) ;

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(ForbidResult));
        }


        [Fact]
        public async Task TenantController_Put_GetReturnsNull_ReturnNotFound()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = tenantId;
            A.CallTo(() => _tenantService.Get(tenantId)).Returns<Tenant?>(null);

            // Create a new ClaimsIdentity for the user
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);

            // Set the controller HttpContext to the new HttpContext
            _controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = await _controller.Put(tenantId, A.Fake<UpdateTenantDto>());

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NotFoundObjectResult));
        }

        [Fact]
        public async Task TenantController_Put_UpdateReturnsNull_ReturnNotFound()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = tenantId;
            var newTenantDto = A.Fake<UpdateTenantDto>();


            A.CallTo(() => _tenantService.Get(tenantId)).Returns<Tenant?>(tenant);
            A.CallTo(() => _tenantService.Update(tenantId, newTenantDto)).Returns<Tenant?>(null);

            // Create a new ClaimsIdentity for the user
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);

            // Set the controller HttpContext to the new HttpContext
            _controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = await _controller.Put(tenantId, newTenantDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NotFoundObjectResult));
        }

        /* Delete Test Functions */
        [Fact]
        public async Task TenantController_Delete_ReturnOk()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = tenantId;

            A.CallTo(() => _tenantService.Get(tenantId)).Returns(tenant);
            A.CallTo(() => _tenantService.Delete(tenant.Id)).Returns(tenantId);
            var controller = new TenantController(_tenantService, _logger);

            // Create a new ClaimsIdentity for the user
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);

            // Set the controller HttpContext to the new HttpContext
            controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = await controller.Delete(tenantId);

            // Assert 
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }


        [Fact]
        public async Task TenantController_Delete_GetReturnsNull_ReturnNotFound()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = tenantId;
            A.CallTo(() => _tenantService.Get(tenantId)).Returns<Tenant?>(null);

            // Create a new ClaimsIdentity for the user
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);

            // Set the controller HttpContext to the new HttpContext
            _controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = await _controller.Delete(tenantId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(NotFoundObjectResult));
        }

        [Fact]
        public async Task TenantController_Delete_DeleteReturnsNull_ReturnBadRequest()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = tenantId;


            A.CallTo(() => _tenantService.Get(tenantId)).Returns<Tenant?>(tenant);
            A.CallTo(() => _tenantService.Delete(tenantId)).Returns<string?>(null);

            // Create a new ClaimsIdentity for the user
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);

            // Set the controller HttpContext to the new HttpContext
            _controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = await _controller.Delete(tenantId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(BadRequestObjectResult));
        }
    }
}