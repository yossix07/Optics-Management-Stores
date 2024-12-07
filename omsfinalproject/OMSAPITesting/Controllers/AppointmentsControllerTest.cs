using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using OMSAPI.Dto.AppointmentsDto;
using OMSAPI.Dto;
using OMSAPI.Models.Appointments;
using OMSAPI.Models.Entities;
using OMSAPI.Services.ServicesInterfaces;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using OMSAPI.Controllers.AppointmentsControllers;

namespace OMSAPITesting.Controllers
{
    public class AppointmentsControllerTests
    {
        private readonly AppointmentsController _controller;
        private readonly ILogger<AppointmentsController> _logger;
        private readonly IAppointmentServices _appointmentServices;
        private readonly IUserServices _userServices;
        private readonly IAppointmentSettingsServices _appointmentSettingsServices;

        public AppointmentsControllerTests()
        {
            _logger = A.Fake<ILogger<AppointmentsController>>();
            _appointmentServices = A.Fake<IAppointmentServices>();
            _userServices = A.Fake<IUserServices>();
            _appointmentSettingsServices = A.Fake<IAppointmentSettingsServices>();
            
            _controller = new AppointmentsController(_logger, _appointmentServices, _userServices, _appointmentSettingsServices);
        }

        [Fact]
        public async Task GetAppointmentsAsync_Returns_Forbid_When_Tenant_Authorization_Fails()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            Tenant tenant = A.Fake<Tenant>();

            // Create a default http context with a tenant
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = await _controller.GetAppointmentsAsync(tenantId, A.Dummy<DateRangeWithStatusDto>());

            // Assert
            Assert.IsType<ForbidResult>(result.Result);
        }

        
        [Fact]
        public async Task GetAppointmentsAsync_Returns_List_Of_Appointment_Slots_When_Tenant_Authorization_Passes()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            Tenant tenant = A.Fake<Tenant>();
            tenant.Id = tenantId;

            // Create a default http context with a tenant
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;

            DateDto startDate = new DateDto(DateTime.Now.Year, 1, 1);
            DateDto endDate = new DateDto(DateTime.Now.AddYears(1).Year, 1, 1);
            var dateRangeWithStatusDto = new DateRangeWithStatusDto
            {
                Start = startDate,
                End = endDate,
                Status = AppointmentStatus.Pending
            };

            var users = A.Fake<List<User>>();
            var user = A.Fake<User>();
            user.Id = "123";
            users.Add(user);
            A.CallTo(() => _userServices.GetAll(tenantId)).Returns(Task.FromResult<List<User>?>(users));

            // create a list of appointments when userId is 123;

            var appointments = A.Fake<List<AppointmentSlotDto>>();
            var appointment = A.Fake<AppointmentSlotDto>();
            appointment.UserId = "123";
            appointments.Add(appointment);

            var dateonly = new DateOnly(startDate.Year, startDate.Month, startDate.Day);
            var expected = A.Fake<Dictionary<DateOnly, List<AppointmentSlotDto>>>();
            expected.Add(dateonly, appointments);

            A.CallTo(() => _appointmentServices.GetAppointmentsByDateAndStatus(tenantId, dateonly, dateonly.AddYears(1), dateRangeWithStatusDto.Status.ToString(), users))
                .Returns(expected);

            // Act
            var result = await _controller.GetAppointmentsAsync(tenantId,dateRangeWithStatusDto );

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnAppointments = Assert.IsType<ActionResult<Dictionary<DateOnly, List<AppointmentSlotDto>>>?>(okResult.Value);
        }
        


        /** GetAppointmentHelper Test Functions */
        [Fact]
        public async Task GetAppointmentHelper_ReturnsBadRequestResult_WhenNoUsersAreFound()
        {
            // Arrange
            A.CallTo(() => _userServices.GetAll(A<string>._)).Returns(Task.FromResult<List<User>?>(null));
            var tenantId = ObjectId.GenerateNewId().ToString();

            DateDto startDate = new DateDto(DateTime.Now.Year,1,1);
            DateDto endDate = new DateDto(DateTime.Now.AddYears(1).Year, 1, 1);
            var dateRangeWithStatusDto = new DateRangeWithStatusDto {
                Start = startDate,
                End = endDate,
                Status = AppointmentStatus.Pending };

            // Act
            var result = await _controller.GetAppointmentHelper(tenantId, dateRangeWithStatusDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetAppointmentHelper_ReturnsBadRequestResult_WhenAppointmentsAreNotFound()
        {
            // Arrange
            A.CallTo(() => _appointmentServices.GetAppointmentsByDateAndStatus(A<string>._, A<DateOnly>._, A<DateOnly>._, A<string>._, A<List<User>>._))
                .Returns<ActionResult<Dictionary<DateOnly, List<AppointmentSlotDto>>>?>((null));
            A.CallTo(() => _userServices.GetAll(A<string>._)).Returns(new List<User>());
            
            var tenantId = ObjectId.GenerateNewId().ToString(); 
            DateDto startDate = new DateDto(DateTime.Now.Year, 1, 1);
            DateDto endDate = new DateDto(DateTime.Now.AddYears(1).Year, 1, 1);
            var dateRangeWithStatusDto = new DateRangeWithStatusDto
            {
                Start = startDate,
                End = endDate,
                Status = AppointmentStatus.Pending
            };

            // Act
            var result = await _controller.GetAppointmentHelper(tenantId, dateRangeWithStatusDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }




        /* GetAllUserAppointmentsAsync Test Functions*/

        [Fact]
        public async Task GetAllUserAppointmentsAsync_ReturnsBadRequest_WhenUserIsNull()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            User user = A.Fake<User>();
            user.Id = "123";

            var httpContext = Utils.Utils.CreateDefaultHttpContext(user);
            _controller.ControllerContext.HttpContext = httpContext;

            A.CallTo(() => _userServices.GetById(tenantId, user.Id)).Returns<User?>(null);

            // Act
            var result = await _controller.GetAllUserAppointmentsAsync(tenantId, user.Id);

            // Assert
            A.CallTo(() => _userServices.GetById(tenantId, user.Id)).MustHaveHappened();

            Assert.IsType<BadRequestObjectResult>(result?.Result);
        }


        [Fact]
        public async Task GetAllUserAppointmentsAsync_ReturnsBadRequest_WhenAppointmentsIsNull()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var user = A.Fake<User>();
            user.Id="123";

            // Create a new HttpContext and add the user to it
            var httpContext = Utils.Utils.CreateDefaultHttpContext(user);
            _controller.ControllerContext.HttpContext = httpContext;

            A.CallTo(() => _userServices.GetById(tenantId, user.Id)).Returns<User?>(user);
            A.CallTo(() => _appointmentServices.GetAllUserAppointments(tenantId, user))
                .Returns<ActionResult<Dictionary<DateOnly,List<AppointmentSlotDto>>>?>(null);

            // Act
            var result = await _controller.GetAllUserAppointmentsAsync(tenantId, user.Id);

            // Assert
            A.CallTo(() => _userServices.GetById(tenantId, user.Id)).MustHaveHappened();
            A.CallTo(() => _appointmentServices.GetAllUserAppointments(tenantId, user)).MustHaveHappened();
            Assert.IsType<BadRequestObjectResult>(result?.Result);
        }


        [Fact]
        public async Task GetAllUserAppointmentsAsync_ReturnsForbid_WhenAuthorizationFails()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var user1 = A.Fake<User>();
            user1.Id = "123";
            var user2 = A.Fake<User>();
            user2.Id = "456";

            // Create a new HttpContext for different user.
            var httpContext = Utils.Utils.CreateDefaultHttpContext(user2);
            _controller.ControllerContext.HttpContext = httpContext;

            A.CallTo(() => _userServices.GetById(tenantId, user1.Id)).Returns<User?>(user1);

            // Act
            var result = await _controller.GetAllUserAppointmentsAsync(tenantId, user1.Id);

            // Assert
            A.CallTo(() => _userServices.GetById(tenantId, user1.Id)).MustNotHaveHappened();
            Assert.IsType<ForbidResult>(result?.Result);
        }

        [Fact]
        public async Task GetAllUserAppointmentsAsync_ReturnsOk()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var user = A.Fake<User>();
            user.Id = "123";

            // Create a new HttpContext and add the user to it
            var httpContext = Utils.Utils.CreateDefaultHttpContext(user);
            _controller.ControllerContext.HttpContext = httpContext;

            var appointments = new Dictionary<DateOnly, List<AppointmentSlotDto>>();
            A.CallTo(() => _userServices.GetById(tenantId, user.Id)).Returns(user);
            A.CallTo(() => _appointmentServices.GetAllUserAppointments(tenantId, user)).Returns(appointments);

            // Act
            var result = await _controller.GetAllUserAppointmentsAsync(tenantId, user.Id);

            // Assert
            A.CallTo(() => _userServices.GetById(tenantId, user.Id)).MustHaveHappened();
            A.CallTo(() => _appointmentServices.GetAllUserAppointments(tenantId, user)).MustHaveHappened();
            Assert.IsType<OkObjectResult>(result?.Result);
        }



        /*GetAppointmentByIdAsync Test Functions*/
        [Fact]
        public async Task GetAppointmentByIdAsync_ReturnsBadRequest_WhenNoAppointmentsFound()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var appointmentId = "123";

            A.CallTo(() => _appointmentServices.GetAppointmentById(tenantId, appointmentId))
                .Returns(Task.FromResult<Dictionary<DateOnly,AppointmentSlotDto>?>(null));

            // Act
            var result = await _controller.GetAppointmentByIdAsync(tenantId, appointmentId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetAppointmentByIdAsync_ReturnsOk_WhenAppointmentsFound()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var appointmentId = "123";
            var appointment1 = A.Fake<AppointmentSlotDto>();
            var appointment2 = A.Fake<AppointmentSlotDto>();
            appointment1.Id = appointmentId;
            var appointments = new Dictionary<DateOnly, AppointmentSlotDto>
            {
                { new DateOnly(2022, 1, 1), appointment1  },
                { new DateOnly(2022, 1, 2), appointment2  }
            };

            
            A.CallTo(() => _appointmentServices.GetAppointmentById(tenantId, appointmentId)).Returns(appointments);

            // Act
            var result = await _controller.GetAppointmentByIdAsync(tenantId, appointmentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var resultValue = Assert.IsAssignableFrom<Dictionary<DateOnly,AppointmentSlotDto>>(okResult.Value);
            Assert.Equal(appointments, resultValue);
        }

        /*CreateNewAppointment Test Functions */
        [Fact]
        public async Task CreateNewAppointment_ReturnsOk_WhenCreated()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var user = A.Fake<User>();
            user.Id = "123";

            // Create a new HttpContext and add the user to it
            var httpContext = Utils.Utils.CreateDefaultHttpContext(user);
            _controller.ControllerContext.HttpContext = httpContext;


            var appointmentCreateDto = A.Fake<CreateAppointmentDto>();
            appointmentCreateDto.UserId = user.Id;


            A.CallTo(() => _appointmentServices.CreateAppointment(tenantId, appointmentCreateDto)).Returns(Task.FromResult(true));

            // Act
            var result = await _controller.CreateNewAppointmentAsync(tenantId, appointmentCreateDto);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.Equal($"Appointment created for user {user.Id}", okResult?.Value);
        }


        [Fact]
        public async Task CreateNewAppointment_ReturnsForbid_WhenAuthorizationFails()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var user1 = A.Fake<User>();
            user1.Id = "123";
            var user2 = A.Fake<User>();
            user2.Id = "456";

            // Create a new HttpContext for different user.
            var httpContext = Utils.Utils.CreateDefaultHttpContext(user2);
            _controller.ControllerContext.HttpContext = httpContext;
            
            // Create a new appointment for user1
            var appointmentCreateDto = A.Fake<CreateAppointmentDto>();
            appointmentCreateDto.UserId = user1.Id;

            // Act
            var result = await _controller.CreateNewAppointmentAsync(tenantId, appointmentCreateDto);

            // Assert
            Assert.IsType<ForbidResult>(result.Result);
        }


        [Fact]
        public async Task CreateNewAppointment_ReturnsBadRequest_WhenCreatedFails()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var user = A.Fake<User>();
            user.Id = "123";

            // Create a new HttpContext and add the user to it
            var httpContext = Utils.Utils.CreateDefaultHttpContext(user);
            _controller.ControllerContext.HttpContext = httpContext;


            var appointmentCreateDto = A.Fake<CreateAppointmentDto>();
            appointmentCreateDto.UserId = user.Id;

            A.CallTo(() => _appointmentServices.CreateAppointment(tenantId, appointmentCreateDto)).Returns(Task.FromResult(false));

            // Act
            var result = await _controller.CreateNewAppointmentAsync(tenantId, appointmentCreateDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);

            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.Equal($"Faild to create appointment {appointmentCreateDto.AppointmentId}", badRequestResult?.Value);
        }



        /* DeleteAppointmentAsync Test Functions*/
        [Fact]
        public async Task DeleteAppointmentAsync_WithValidData_ShouldReturnOk()
        {
            //Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var user = A.Fake<User>();
            user.Id = "123";
            string appointmentId = "appointmentId";

            // Create a new HttpContext and add the user to it
            var httpContext = Utils.Utils.CreateDefaultHttpContext(user);
            _controller.ControllerContext.HttpContext = httpContext;

            A.CallTo(() => _appointmentServices.DeleteAppointment(tenantId,appointmentId))
                .Returns(Task.FromResult(true));

            //Act
            var result = await _controller.DeleteAppointmentAsync(tenantId, appointmentId, user.Id);

            //Assert
            Assert.IsType<OkResult>(result);
        }


        [Fact]
        public async Task DeleteAppointmentAsync_WithInvalidData_ShouldReturnForbid()
        {   
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var user1 = A.Fake<User>();
            user1.Id = "123";
            var user2 = A.Fake<User>();
            user2.Id = "456";
            string appointmentId = "appointmentId";

            // Create a new HttpContext for different user.
            var httpContext = Utils.Utils.CreateDefaultHttpContext(user2);
            _controller.ControllerContext.HttpContext = httpContext;

            //Act
            var result = await _controller.DeleteAppointmentAsync(tenantId, appointmentId, user1.Id);

            //Assert
            Assert.IsType<ForbidResult>(result);
        }


        [Fact]
        public async Task DeleteAppointmentAsync_WithInvalidAppointmentId_ShouldReturnBadRequest()
        {
            //Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var user = A.Fake<User>();
            user.Id = "123";
            string appointmentId = "appointmentId";

            // Create a new HttpContext and add the user to it
            var httpContext = Utils.Utils.CreateDefaultHttpContext(user);
            _controller.ControllerContext.HttpContext = httpContext;

            A.CallTo(() => _appointmentServices.DeleteAppointment(tenantId, appointmentId))
                           .Returns(Task.FromResult(false));

            //Act
            var result = await _controller.DeleteAppointmentAsync(tenantId, appointmentId, user.Id);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }

}
