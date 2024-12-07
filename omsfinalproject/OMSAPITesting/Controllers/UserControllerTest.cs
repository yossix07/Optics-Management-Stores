using OMSAPI.Models.Entities;
using OMSAPI.Services.ServicesInterfaces;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using FluentAssertions;
using OMSAPI.Dto.EntitiesDto;
using System.Net;
using OMSAPI.Controllers.EntitiesControllers;

namespace OMSAPITesting.Controllers
{
    public class UserControllerTest
    {
        private readonly IUserServices _userService;
        private readonly ILogger<UserController> _logger;
        private readonly UserController _controller;

        public UserControllerTest()
        {
            _userService = A.Fake<IUserServices>();
            _logger = A.Fake<ILogger<UserController>>();
            _controller = new UserController(_userService, _logger);
        }

        /* GetAll Test Functions */
        [Fact]
        public async Task UserController_GetAll_ReturnsOk()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var user = A.Fake<User>();
            user.Id = "123456789";
            var list = new List<User> { user };
            A.CallTo(() => _userService.GetAll(tenantId)).Returns(list);

            // Create a new ClaimsIdentity for the user
            var httpContext = Utils.Utils.CreateDefaultHttpContext(user);

            // Set the controller HttpContext to the new HttpContext
            _controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = await _controller.GetAll(tenantId);

            // Assert
            result.Result.Should().NotBeNull();
            result.Result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async Task UserController_GetAll_ReturnsForbid()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = "123";
            A.CallTo(() => _userService.GetAll(A<string>._)).Returns<List<User>?>(null);

            // Create httpContext
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = await _controller.GetAll(tenantId);

            // Assert
            A.CallTo(() => _userService.GetAll(tenantId)).MustNotHaveHappened();
            Assert.IsType<ForbidResult>(result.Result);
        }


        [Fact]
        public async Task UserController_GetAll_ReturnsBadRequest()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = tenantId;

            A.CallTo(() => _userService.GetAll(A<string>._)).Returns<List<User>?>(null);

            // Create httpContext
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = await _controller.GetAll(tenantId);

            // Assert
            A.CallTo(() => _userService.GetAll(tenantId)).MustHaveHappenedOnceExactly();
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        /* Get Test Functions */
        [Fact]
        public async Task UserController_Get_ReturnOk()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var user = A.Fake<User>();
            user.Id = "123456789";
            A.CallTo(() => _userService.GetById(tenantId, user.Id)).Returns(user);
            var controller = new UserController(_userService, _logger);

            // Create a new ClaimsIdentity for the user
            var httpContext = Utils.Utils.CreateDefaultHttpContext(user);

            // Set the controller HttpContext to the new HttpContext
            controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = await controller.Get(tenantId, user.Id);

            // Assert
            result.Result.Should().NotBeNull();
            result.Result.Should().BeOfType(typeof(OkObjectResult));
        }

        
        [Fact]
        public async Task UserController_Get_ReturnsForbid()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = "123";
            var user = A.Fake<User>();
            user.Id = "456";

            A.CallTo(() => _userService.GetById(tenantId, user.Id)).Returns<User?>(null);

            // Create httpContext
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = await _controller.Get(tenantId, user.Id);

            // Assert
            A.CallTo(() => _userService.GetById(tenantId, user.Id)).MustNotHaveHappened();
            Assert.IsType<ForbidResult>(result.Result);
        }


        [Fact]
        public async Task UserController_Get_ReturnsNotFound()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var user = A.Fake<User>();
            user.Id = "123";

            // Create httpContext
            var httpContext = Utils.Utils.CreateDefaultHttpContext(user);
            _controller.ControllerContext.HttpContext = httpContext;

            A.CallTo(() => _userService.GetById(tenantId, user.Id)).Returns<User?>(null);

            // Act
            var result = await _controller.Get(tenantId, user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NotFoundObjectResult>(result.Result);

            var notFoundResult = result.Result as NotFoundObjectResult;

            Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult?.StatusCode);
        }


        /* Put Test Functions */
        [Fact]
        public async Task UserCollector_Put_ReturnOk()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var user = A.Fake<User>();
            user.Id = "123456789";
            user.Name = "oldUser";
            var newUserDto = A.Fake<UpdateUserDto>();
            newUserDto.Email = "test@gmail.com";
            newUserDto.Name = "newUserName";
            newUserDto.PhoneNumber = "054-21212121";

            A.CallTo(() => _userService.GetById(tenantId, user.Id)).Returns(user);
            A.CallTo(() => _userService.Update(tenantId, user.Id, newUserDto)).Returns(user);
            var controller = new UserController(_userService, _logger);

            // Create a new ClaimsIdentity for the user
            var httpContext = Utils.Utils.CreateDefaultHttpContext(user);

            // Set the controller HttpContext to the new HttpContext
            controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = await controller.Put(tenantId, user.Id, newUserDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkResult));
        }


        [Fact]
        public async Task UserController_Put_ReturnForbid()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = "123";
            var user = A.Fake<User>();
            user.Id = "456";

            var updateUser = A.Fake<UpdateUserDto>();

            A.CallTo(() => _userService.GetById(tenantId, user.Id)).Returns<User?>(null);

            // Create httpContext
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = await _controller.Put(tenantId, user.Id, updateUser);

            // Assert
            A.CallTo(() => _userService.GetById(tenantId, user.Id)).MustNotHaveHappened();
            Assert.IsType<ForbidResult>(result);
        }


        [Fact]
        public async Task UserController_Put_GetByIdReturnsNull_ReturnBadRequest()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var user = A.Fake<User>();
            user.Id = "123";

            // Create httpContext
            var httpContext = Utils.Utils.CreateDefaultHttpContext(user);
            _controller.ControllerContext.HttpContext = httpContext;

            A.CallTo(() => _userService.GetById(tenantId, user.Id)).Returns<User?>(null);

            // Act
            var result = await _controller.Put(tenantId, user.Id, A.Fake<UpdateUserDto>());

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UserController_Delete_UpdateReturnsNull_ReturnNotFound()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var user = A.Fake<User>();
            user.Id = "123";

            var updateUser = A.Fake<UpdateUserDto>();

            // Create httpContext
            var httpContext = Utils.Utils.CreateDefaultHttpContext(user);
            _controller.ControllerContext.HttpContext = httpContext;

            A.CallTo(() => _userService.GetById(tenantId, user.Id)).Returns<User?>(user);
            A.CallTo(() => _userService.Update(tenantId, user.Id, updateUser)).Returns<User?>(null);

            // Act
            var result = await _controller.Put(tenantId, user.Id, updateUser);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        /* Delete Test Functions */
        [Fact]
        public async Task UserController_Delete_ReturnOk()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var user = A.Fake<User>();
            user.Id = "123456789";


            A.CallTo(() => _userService.GetById(tenantId, user.Id)).Returns(user);
            A.CallTo(() => _userService.Delete(tenantId, user.Id)).Returns(user.Id);
            var controller = new UserController(_userService, _logger);

            // Create a new ClaimsIdentity for the user
            var httpContext = Utils.Utils.CreateDefaultHttpContext(user);

            // Set the controller HttpContext to the new HttpContext
            controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = await controller.Delete(tenantId, user.Id);

            // Assert 
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkResult));
        }

        [Fact]
        public async Task UserController_Delete_ReturnForbid()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = "123";
            var user = A.Fake<User>();
            user.Id = "456";

            A.CallTo(() => _userService.GetById(tenantId, user.Id)).Returns<User?>(null);

            // Create httpContext
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = await _controller.Delete(tenantId, user.Id);

            // Assert
            A.CallTo(() => _userService.GetById(tenantId, user.Id)).MustNotHaveHappened();
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task UserController_Delete_GetByIdReturnsNull_ReturnNotFound()
        { 
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var user = A.Fake<User>();
            user.Id = "123";

            // Create httpContext
            var httpContext = Utils.Utils.CreateDefaultHttpContext(user);
            _controller.ControllerContext.HttpContext = httpContext;

            A.CallTo(() => _userService.GetById(tenantId, user.Id)).Returns<User?>(null);

            // Act
            var result = await _controller.Delete(tenantId, user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task UserController_Delete_DeleteReturnsNull_ReturnNotFound()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var user = A.Fake<User>();
            user.Id = "123";

            // Create httpContext
            var httpContext = Utils.Utils.CreateDefaultHttpContext(user);
            _controller.ControllerContext.HttpContext = httpContext;

            A.CallTo(() => _userService.GetById(tenantId, user.Id)).Returns<User?>(user);
            A.CallTo(() => _userService.Delete(tenantId, user.Id)).Returns<string?>(null);

            // Act
            var result = await _controller.Delete(tenantId, user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NotFoundObjectResult>(result);
        }

       


        


    }
}