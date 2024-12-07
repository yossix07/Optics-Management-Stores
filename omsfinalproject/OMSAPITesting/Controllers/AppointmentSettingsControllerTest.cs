using OMSAPI.Services.ServicesInterfaces;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using OMSAPI.Models.Appointments;
using OMSAPI.Dto.AppointmentsDto;
using OMSAPI.Dto;
using OMSAPI.Controllers.AppointmentsControllers;

namespace OMSAPITesting.Controllers
{
    public class AppointmentSettingsControllerTest
    {
        private readonly ILogger<AppointmentSettingsController> _logger;
        private readonly IAppointmentSettingsServices _appointmentSettingsServices;
        private readonly ITenantServices _tenantServices;
        private readonly AppointmentSettingsController _controller;


        public AppointmentSettingsControllerTest()
        {
            _logger = A.Fake<ILogger<AppointmentSettingsController>>();
            _appointmentSettingsServices = A.Fake<IAppointmentSettingsServices>();
            _tenantServices = A.Fake<ITenantServices>();
            _controller = new AppointmentSettingsController(_logger, _appointmentSettingsServices, _tenantServices);
        }

        /** GetAppointmentTypes Function Tests */
        //TODO: Add test for GetAppointmentTypes

        /* CreateNewAppointment Function Tests */
        [Fact]
        public async Task CreateNewAppointmentType_ValidInput_ReturnsOk()
        {
            // Arrange
            var tenant = Utils.Utils.createCustomAppointmentSettings();
            var tenantId = tenant.Id;

            var appointmentType = new AppointmentType { TypeName = "type1" };

            A.CallTo(() => _appointmentSettingsServices.GetAppointmentTypesByName(tenantId, appointmentType.TypeName))
                .Returns(Task.FromResult<AppointmentType?>(appointmentType));

            A.CallTo(() => _appointmentSettingsServices.CreateAppointmentType(tenantId, appointmentType))
                .Returns(Task.FromResult<bool?>(true));

            // Create a new ClaimsIdentity for the user and Set the controller HttpContext to the new HttpContext
            var httpContext = Utils.Utils.CreateDefaultHttpContext(tenant);
            _controller.ControllerContext.HttpContext = httpContext;
            // Act
            var result = await _controller.CreateNewAppointmentType(tenantId, appointmentType);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<AppointmentType>(okResult.Value);
            Assert.Equal(appointmentType, returnValue);
        }

        [Fact]
        public async Task CreateNewAppointmentType_AlreadyExists_ReturnsBadRequest()
        {
            // Arrange
            var tenant = Utils.Utils.createCustomAppointmentSettings();
            var tenantId = tenant.Id;
            var appointmentType = new AppointmentType { TypeName = "type1" };

            A.CallTo(() => _appointmentSettingsServices.CreateAppointmentType(tenantId, appointmentType))
                           .Returns(Task.FromResult<bool?>(null));

            // Act
            var result = await _controller.CreateNewAppointmentType(tenantId, appointmentType);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public async Task CreateNewAppointmentType_Failed_ReturnsBadRequest()
        {
            // Arrange
            var tenant = Utils.Utils.createCustomAppointmentSettings();
            var tenantId = tenant.Id;
            var appointmentType = new AppointmentType { TypeName = "type1" };

            A.CallTo(() => _appointmentSettingsServices.CreateAppointmentType(tenantId, appointmentType))
                .Returns(Task.FromResult<bool?>(false));

            // Act
            var result = await _controller.CreateNewAppointmentType(tenantId, appointmentType);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        /* UpdateAppointmentType Function Tests */
        [Fact]
        public async Task PutAppointmentType_ValidInput_ReturnsOk()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var appointmentType = new AppointmentType { TypeName = "TestType" };
            A.CallTo(() => _appointmentSettingsServices.PutAppointmentType(tenantId, appointmentType)).Returns(Task.FromResult(true));

            // Act
            var result = await _controller.PutAppointmentType(tenantId, appointmentType) as OkObjectResult;

            // Assert
            A.CallTo(() => _appointmentSettingsServices.PutAppointmentType(tenantId, appointmentType)).MustHaveHappened();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task PutAppointmentType_InvalidInput_ReturnsBadRequest()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var appointmentType = new AppointmentType { TypeName = "TestType" };
            A.CallTo(() => _appointmentSettingsServices.PutAppointmentType(tenantId, appointmentType)).Returns(Task.FromResult(false));

            // Act
            var result = await _controller.PutAppointmentType(tenantId, appointmentType) as BadRequestObjectResult;

            // Assert
            A.CallTo(() => _appointmentSettingsServices.PutAppointmentType(tenantId, appointmentType)).MustHaveHappened();
            Assert.IsType<BadRequestObjectResult>(result);
        }


        /** DeleteAppointmentType Function Tests */
        [Fact]
        public async Task DeleteAppointmentType_WhenValidData_ShouldReturnOk()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var appointmentTypeName = "Type1";
            A.CallTo(() => _appointmentSettingsServices.DeleteAppointmentType(tenantId, appointmentTypeName)).Returns(true);

            // Act
            var result = await _controller.DeleteAppointmentType(tenantId, appointmentTypeName);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteAppointmentType_WhenInvalidData_ShouldReturnBadRequest()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var appointmentTypeName = "Type1";
            A.CallTo(() => _appointmentSettingsServices.DeleteAppointmentType(tenantId, appointmentTypeName)).Returns(false);

            // Act
            var result = await _controller.DeleteAppointmentType(tenantId, appointmentTypeName);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        /** GetDaysOff Function Tests */
        [Fact]
        public async Task GetDaysOff_WithValidTenantId_ReturnsOkResult()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var daysOff = new List<Holiday> { new Holiday("Test", DateOnly.FromDateTime(DateTime.Now)) };
            A.CallTo(() => _appointmentSettingsServices.GetAllDaysOff(tenantId)).Returns(Task.FromResult<List<Holiday>?>(daysOff));

            // Act
            var result = await _controller.GetDaysOff(tenantId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var model = Assert.IsAssignableFrom<List<Holiday>?>(okResult.Value);
            Assert.Equal(daysOff, model);
        }

        [Fact]
        public async Task GetDaysOff_WithInvalidTenantId_ReturnsBadRequest()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            A.CallTo(() => _appointmentSettingsServices.GetAllDaysOff(tenantId)).Returns(Task.FromResult<List<Holiday>?>(null));

            // Act
            var result = await _controller.GetDaysOff(tenantId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }


        /** CreateNewDayOff Function Tests */
       
        [Fact]
        public async Task CreateNewDayOff_ValidHolidayDto_ReturnsOk()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            HolidayDto? holidayDto = new HolidayDto(2030, 1, 1);
            Holiday holiday = new Holiday("test_holiday", holidayDto.convertToDateOnly());
            A.CallTo(() => _appointmentSettingsServices.CreateDayOff(tenantId, A<Holiday>.Ignored))
                .Returns(true);

            // Act
            var result = await _controller.CreateNewDayOff(tenantId, holidayDto);

            // Assert
            A.CallTo(() => _appointmentSettingsServices.CreateDayOff(tenantId, A<Holiday>.That.Matches(h => h.Name == holidayDto.Name && h.Date == holidayDto.convertToDateOnly())))
                .MustHaveHappenedOnceExactly();
            Assert.IsType<OkObjectResult>(result);
        }
       
        [Fact]
        public async Task CreateNewDayOff_InvalidHolidayDto_ReturnsBadRequest()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            HolidayDto? holidayDto = new HolidayDto(2030, 1, 1);
            Holiday holiday = new Holiday("test_holiday", holidayDto.convertToDateOnly());
            A.CallTo(() => _appointmentSettingsServices.CreateDayOff(tenantId, A<Holiday>.Ignored))
                .Returns(false);

            // Act
            var result = await _controller.CreateNewDayOff(tenantId, holidayDto);

            // Assert
            A.CallTo(() => _appointmentSettingsServices.CreateDayOff(tenantId, A<Holiday>.That.Matches(h => h.Name == holidayDto.Name)))
                .MustHaveHappenedOnceExactly();
            Assert.IsType<BadRequestObjectResult>(result);
        }
        
        [Fact]
        public async Task CreateNewDayOff_ServiceException_ReturnsBadRequest()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            HolidayDto? holidayDto = new HolidayDto(2030, 1, 1);
            Holiday holiday = new Holiday("test_holiday", holidayDto.convertToDateOnly());
            A.CallTo(() => _appointmentSettingsServices.CreateDayOff(tenantId, A<Holiday>.Ignored))
                .Throws(new Exception("test_exception_message"));

            // Act
            var result = await _controller.CreateNewDayOff(tenantId, holidayDto);

            // Assert
            A.CallTo(() => _appointmentSettingsServices.CreateDayOff(tenantId, A<Holiday>.That.Matches(h => h.Name == holidayDto.Name && h.Date == holidayDto.convertToDateOnly())))
                .MustHaveHappenedOnceExactly();
            Assert.IsType<BadRequestObjectResult>(result);
        }


        /** DeleteDayOff Function Tests */

        [Fact]
        public async Task DeleteDayOff_ReturnsBadRequest_WhenAppointmentTypeListIsEmpty()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            DateDto dateDto = new DateDto(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            A.CallTo(() => _appointmentSettingsServices.DeleteDayOffByDate(tenantId, A<DateOnly>.Ignored)).Returns(false);

            // Act
            var result = await _controller.DeleteDayOff(tenantId, dateDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteDayOff_ReturnsOk_WhenAppointmentTypeListIsDeleted()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            DateDto dateDto = new DateDto(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            A.CallTo(() => _appointmentSettingsServices.DeleteDayOffByDate(tenantId, A<DateOnly>.Ignored)).Returns(true);

            // Act
            var result = await _controller.DeleteDayOff(tenantId, dateDto);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        /** GetAvailableBlocks Function Tests */
        [Fact]
        public async Task GetAvailableBlocks_ReturnsAllBlocks()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            TimeOnly startTime = new TimeOnly(8, 0);
            TimeOnly endTime = new TimeOnly(17, 0);
            List<AppointmentsAvailableBlock> blocks = new List<AppointmentsAvailableBlock>
            {
                new AppointmentsAvailableBlock(startTime, endTime , DayOfWeek.Monday),
                new AppointmentsAvailableBlock(startTime, endTime , DayOfWeek.Tuesday)
            };
            A.CallTo(() => _appointmentSettingsServices.GetAllAvailableBlocks(tenantId)).Returns(blocks);

            // Act
            var result = await _controller.GetAvailableBlocks(tenantId);

            // Assert
            A.CallTo(() => _appointmentSettingsServices.GetAllAvailableBlocks(tenantId)).MustHaveHappenedOnceExactly();
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.Equal(blocks, okResult?.Value);
        }

        [Fact]
        public async Task GetAvailableBlocks_ReturnsBadRequest_WhenServiceReturnsNull()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            A.CallTo(() => _appointmentSettingsServices.GetAllAvailableBlocks(tenantId)).Returns(Task.FromResult<List<AppointmentsAvailableBlock>?>(null));

            // Act
            var result = await _controller.GetAvailableBlocks(tenantId);

            // Assert
            A.CallTo(() => _appointmentSettingsServices.GetAllAvailableBlocks(tenantId)).MustHaveHappenedOnceExactly();
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        /** CreateNewBlock Function Tests */

        [Fact]
        public async Task CreateNewBlock_WithInvalidAvailableBlock_ReturnsBadRequest()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var dto = new AppointmentsAvailableBlockDto(8, 0, 17, 0, DayOfWeek.Monday);

            // Act
            var result = await _controller.CreateNewBlock(tenantId, dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateNewBlock_ValidAvailableBlock_CallsServiceMethodAndReturnsOkResult()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var dto = new AppointmentsAvailableBlockDto(8, 0, 17, 0, DayOfWeek.Monday);
            var availableBlock = dto.CreateAvailableBlock();
            A.CallTo(() => _appointmentSettingsServices.ValidateAvailableBlock(A<string>._, A<AppointmentsAvailableBlock>._)).Returns(true);
            A.CallTo(() => _appointmentSettingsServices.CreateAvailableBlock(A<string>._, A<AppointmentsAvailableBlock>._)).Returns(Task.FromResult(true));

            // Act
            var result = await _controller.CreateNewBlock(tenantId, dto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task CreateNewBlock_ServiceValidationFails_ReturnsBadRequest()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var dto = new AppointmentsAvailableBlockDto(8, 0, 17, 0, DayOfWeek.Monday);
            var availableBlock = dto.CreateAvailableBlock();
            A.CallTo(() => _appointmentSettingsServices.ValidateAvailableBlock(A<string>._, A<AppointmentsAvailableBlock>._)).Returns(false);

            if (availableBlock == null)
            {
                throw new Exception("availableBlock is null");
            }

            // Act
            var result = await _controller.CreateNewBlock(tenantId, dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateNewBlock_ServiceReturnsFalse_ReturnsBadRequest()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var dto = new AppointmentsAvailableBlockDto(8, 0, 17, 0, DayOfWeek.Monday);
            var availableBlock = dto.CreateAvailableBlock();

            if (availableBlock == null)
            {
                throw new Exception("availableBlock is null");
            }

            A.CallTo(() => _appointmentSettingsServices.ValidateAvailableBlock(A<string>.Ignored, A<AppointmentsAvailableBlock>.Ignored)).Returns(true);
            A.CallTo(() => _appointmentSettingsServices.CreateAvailableBlock(A<string>.Ignored, A<AppointmentsAvailableBlock>.Ignored)).Returns(Task.FromResult(false));

            // Act
            var result = await _controller.CreateNewBlock(tenantId, dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }


        /** DeleteAvailableBlock Function Tests */
        [Fact]
        public async Task DeleteAvailableBlock_ReturnsBadRequest_WhenStartTimeIsAfterEndTime()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var availableBlockDto = new AppointmentsAvailableBlockDto(8, 0, 17, 0, DayOfWeek.Monday);
            var availableBlock = availableBlockDto.CreateAvailableBlock();

            if (availableBlock == null)
            {
                throw new Exception("availableBlock is null");
            }

            // Act
            var result = await _controller.DeleteAvailableBlock(tenantId, availableBlockDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteAvailableBlock_ReturnsBadRequest_WhenDeleteAvailableBlockReturnsFalse()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var availableBlockDto = new AppointmentsAvailableBlockDto(8, 0, 17, 0, DayOfWeek.Monday);
            var availableBlock = availableBlockDto.CreateAvailableBlock();
            if (availableBlock == null)
            {
                throw new Exception("availableBlock is null");
            }

            A.CallTo(() => _appointmentSettingsServices.DeleteAvailableBlock(tenantId,availableBlock)).Returns(Task.FromResult(false));

            // Act
            var result = await _controller.DeleteAvailableBlock(tenantId, availableBlockDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteAvailableBlock_ReturnsOkResult_WhenDeleteAvailableBlockReturnsTrue()
        {
            var tenantId = ObjectId.GenerateNewId().ToString();
            var availableBlockDto = new AppointmentsAvailableBlockDto(8, 0, 17, 0, DayOfWeek.Monday);

            // Arrange
            A.CallTo(() => _appointmentSettingsServices.DeleteAvailableBlock(A<string>.Ignored, A<AppointmentsAvailableBlock>.Ignored)).Returns(Task.FromResult(true));


            // Act
            var result = await _controller.DeleteAvailableBlock(tenantId, availableBlockDto);

            // Assert
            Assert.IsType<OkResult>(result);
        }


        /** GetSlotDuration Function Tests */
        [Fact]
        public async Task GetSlotDuration_ValidId_ReturnsOk()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            TimeSpan expectedSlotDuration = new TimeSpan(0, 15, 0);
            A.CallTo(() => _appointmentSettingsServices.GetSlotDuration(A<string>._)).Returns(Task.FromResult<TimeSpan?>(expectedSlotDuration));

            // Act
            var result = await _controller.GetSlotDuration(tenantId);

            // Assert
            A.CallTo(() => _appointmentSettingsServices.GetSlotDuration(tenantId)).MustHaveHappenedOnceExactly();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetSlotDuration_InvalidId_ReturnsBadRequestObjectResult()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            TimeSpan expectedSlotDuration = new TimeSpan(0, 15, 0);
            A.CallTo(() => _appointmentSettingsServices.GetSlotDuration(A<string>._)).Returns(Task.FromResult<TimeSpan?>(null));

            // Act
            var result = await _controller.GetSlotDuration(tenantId);

            // Assert
            A.CallTo(() => _appointmentSettingsServices.GetSlotDuration(tenantId)).MustHaveHappenedOnceExactly();
            Assert.IsType<BadRequestObjectResult>(result);
        }



        /** PutSlotDuration Function Tests */
        [Fact]
        public async Task PutSlotDuration_Returns_BadRequest_If_Duration_Less_Than_10_Minutes()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var duration = new SlotDurationDto { Hours = 0, Minutes = 5 };

            // Act
            var result = await _controller.PutSlotDuration(tenantId, duration);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task PutSlotDuration_Returns_BadRequest_If_Failed_To_Execute_UpdateSlotDuration()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var duration = new SlotDurationDto { Hours = 1, Minutes = 0 };
            A.CallTo(() => _appointmentSettingsServices.UpdateSlotDuration(A<string>._, A<TimeSpan>._)).Returns(Task.FromResult<bool?>(false));

            // Act
            var result = await _controller.PutSlotDuration(tenantId, duration);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task PutSlotDuration_Returns_BadRequest_If_There_Are_Available_Blocks()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var duration = new SlotDurationDto { Hours = 1, Minutes = 0 };
            A.CallTo(() => _appointmentSettingsServices.UpdateSlotDuration(A<string>._, A<TimeSpan>._)).Returns(Task.FromResult<bool?>(null));

            // Act
            var result = await _controller.PutSlotDuration(tenantId, duration);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task PutSlotDuration_Returns_Ok()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            var duration = new SlotDurationDto { Hours = 1, Minutes = 0 };
            A.CallTo(() => _appointmentSettingsServices.UpdateSlotDuration(A<string>._, A<TimeSpan>._)).Returns(Task.FromResult<bool?>(true));

            // Act
            var result = await _controller.PutSlotDuration(tenantId, duration);

            // Assert
            Assert.IsType<OkResult>(result);
        }



        /** GenerateSlots Function Tests */
        [Fact]
        public async Task GenerateSlotsAsync_StartDateGreaterThanEndDate_ShouldReturnBadRequest()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            DateDto startTime = new DateDto(DateTime.Now.AddYears(1).Year, DateTime.Now.Month, DateTime.Now.Day);
            DateDto endTime = new DateDto(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var dateRangeDto = new DateRangeDto(startTime, endTime);

            // Act
            var result = await _controller.GenerateSlotsAsync(tenantId, dateRangeDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GenerateSlotsAsync_ServiceReturnsData_ReturnOk()
        {
            // Arrange
            var tenantId = ObjectId.GenerateNewId().ToString();
            DateTime startDate = DateTime.Now.AddMinutes(15);
            DateTime endDate = DateTime.Now.AddMinutes(30);
            var startTime = new TimeOnly(startDate.Hour, startDate.Minute);
            var endTime = new TimeOnly(endDate.Hour, endDate.Minute);
            var appointmentSlot = new AppointmentSlot
            {
                StartTime = startTime,
                EndTime = endTime
            };

            var date = DateOnly.FromDateTime(startDate);
            var expected = new Dictionary<DateOnly, List<AppointmentSlot>>()
                { { date, new List<AppointmentSlot>() { appointmentSlot } } };          

            var dateRangeDto = new DateRangeDto
            {
                Start = new DateDto(startDate.Year, startDate.Month, startDate.Day),
                End = new DateDto(endDate.Year, endDate.Month, endDate.Day)
            };

            A.CallTo(() => _appointmentSettingsServices.GenerateSlots(tenantId, startDate, endDate, null, null))
                .Returns(Task.FromResult<Dictionary<DateOnly, List<AppointmentSlot>>?>(expected));

            // Act
            var result = await _controller.GenerateSlotsAsync(tenantId, dateRangeDto);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }
    }
}
