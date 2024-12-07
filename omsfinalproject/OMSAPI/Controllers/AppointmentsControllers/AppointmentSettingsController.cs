using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using OMSAPI.Dto;
using OMSAPI.Dto.AppointmentsDto;
using OMSAPI.General;
using OMSAPI.Models.Appointments;
using OMSAPI.Services.ServicesInterfaces;
using System.ComponentModel;

namespace OMSAPI.Controllers.AppointmentsControllers
{
    [Route("api/{tenantId}/[controller]")]
    [ApiController]
    [Authorize(Policy = Roles.Roles.User)]
    public class AppointmentSettingsController : ControllerBase
    {
        private readonly ILogger<AppointmentSettingsController> _logger;
        private readonly IAppointmentSettingsServices _appointmentSettingsServices;
        private readonly ITenantServices _tenantServices;


        public AppointmentSettingsController(ILogger<AppointmentSettingsController> logger, IAppointmentSettingsServices appointmentSettingsServices, ITenantServices tenantServices)
        {
            _logger = logger;
            _appointmentSettingsServices = appointmentSettingsServices;
            _tenantServices = tenantServices;
        }


        /// <summary>
        /// Each tenant has different list of Appointements types. Return list of AppointmentType for given tenant.
        /// </summary>
        [HttpGet("Types/GetAll")]
        public async Task<ActionResult<List<AppointmentType>>> GetAllAppointmentsTypes([FromRoute] string tenantId)
        {
            var tenant = await _tenantServices.Get(tenantId);
            if (tenant == null)
            {
                _logger.LogError($"Failed to execute GetAllAppointmentsTypes for {tenantId}. Tenant not found.");
                return BadRequest($"Tenant was not found".ToJson());
            }

            var appointmentTypeList = _appointmentSettingsServices.GetAllTypes(tenant);
            if (appointmentTypeList == null)
            {
                _logger.LogError($"Failed to execute GetAllTypes for {tenantId}");
                return BadRequest($"Appointment types was not found.".ToJson());
            }
            return Ok(appointmentTypeList);
        }


        /// <summary>
        /// Create new appointment type for tenant with id = tenantId
        /// </summary>
        [HttpPost("Type/Create")]
        public async Task<IActionResult> CreateNewAppointmentType([FromRoute] string tenantId, [FromBody] AppointmentType appointmentType)
        {
            var appointmentTypeList = await _appointmentSettingsServices.CreateAppointmentType(tenantId, appointmentType);
            if (appointmentTypeList == null)
            {
                _logger.LogError($"Failed to execute CreateNewAppointmentType of {appointmentType.TypeName} for tenant {tenantId}.Appointment type name already exists.");
                return BadRequest($"Appointment type name already exists.".ToJson());
            }
            if (appointmentTypeList == false)
            {
                _logger.LogError($"Failed to execute CreateNewAppointmentType of {appointmentType.TypeName} for tenant {tenantId}.");
                return BadRequest($"Failed to create appointment type.".ToJson());
            }
            _logger.LogInformation($"CreateNewAppointmentType {appointmentType.TypeName} for {tenantId} completed successfully.");
            return Ok(appointmentType);
        }

        /// <summary>
        /// Put new appointment type for tenant with id = tenantId.
        /// </summary>
        [HttpPut("Type/Update")]
        public async Task<IActionResult> PutAppointmentType([FromRoute] string tenantId, [FromBody] AppointmentType appointmentType)
        {
            var appointmentTypeList = await _appointmentSettingsServices.PutAppointmentType(tenantId, appointmentType);
            if (appointmentTypeList == false)
            {
                _logger.LogError($"Failed to execute UpdateAppointmentType of {appointmentType.TypeName} for tenant {tenantId}");
                return BadRequest($"Failed to update appointment type.".ToJson());
            }
            return Ok(appointmentType);
        }

        /// <summary>
        /// Delete appointment type for tenant with id = tenantId.
        /// </summary>
        [HttpDelete("Type/Delete")]
        public async Task<IActionResult> DeleteAppointmentType([FromRoute] string tenantId, [FromBody][Description("The name of the appointment type to delete.")] string appointmentTypeName)
        {
            var appointmentTypeList = await _appointmentSettingsServices.DeleteAppointmentType(tenantId, appointmentTypeName);
            if (appointmentTypeList == false)
            {
                _logger.LogError($"Failed to execute DeleteAppointmentType of {appointmentTypeName} for tenant {tenantId}");
                return BadRequest($"Failed to delete appointment type.".ToJson());

            }
            _logger.LogInformation($"DeleteAppointmentType {appointmentTypeName} for {tenantId} completed successfully.");
            return Ok(appointmentTypeName);
        }

        /// <summary>
        /// The function returns all the days off for tenant with id = tenantId
        /// </summary>
        [HttpGet("DayOff/GetAll")]
        public async Task<ActionResult<List<Holiday>>> GetDaysOff([FromRoute] string tenantId)
        {
            var daysOff = await _appointmentSettingsServices.GetAllDaysOff(tenantId);
            if (daysOff == null)
            {
                _logger.LogError($"Failed to execute GetAllTypes for {tenantId}");
                return BadRequest($"Days off list was not found.".ToJson());
            }
            _logger.LogInformation($"GetDaysOff for {tenantId} completed successfully.");
            return Ok(daysOff);
        }


        /// <summary>
        /// Delete Dayoff for tenant with id = tenantId
        /// </summary>
        [HttpPost("DayOff/Create")]
        public async Task<IActionResult> CreateNewDayOff([FromRoute] string tenantId, [FromBody] HolidayDto holidayDto)
        {
            try
            {
                DateOnly date = holidayDto.convertToDateOnly();
                Holiday holiday = new Holiday(holidayDto.Name, date);

                var appointmentTypeList = await _appointmentSettingsServices.CreateDayOff(tenantId, holiday);
                if (appointmentTypeList == false)
                {
                    _logger.LogError($"Failed to execute CreateNewDayoff of {holiday.Name} for tenant {tenantId}");
                    return BadRequest($"Failed to create day off.".ToJson());
                }
                _logger.LogInformation($"Day off on {holiday.Date} Created");
                return Ok(holiday);
            }
            catch
            {
                _logger.LogError($"Failed to execute CreateNewDayoff of {holidayDto.Name} for tenant {tenantId}");
                return BadRequest($"Failed to create day off.".ToJson());
            }
        }


        /// <summary>
        /// Create new Dayoff for tenant with id = tenantId
        /// </summary>
        [HttpDelete("DayOff/Delete")]
        public async Task<IActionResult> DeleteDayOff([FromRoute] string tenantId, [FromBody] DateDto dateDto)
        {
            DateOnly date = dateDto.convertToDateOnly();
            var appointmentTypeList = await _appointmentSettingsServices.DeleteDayOffByDate(tenantId, date);
            if (appointmentTypeList == false)
            {
                _logger.LogError($"Failed to execute DeleteDayOff of {date} for tenant {tenantId}");
                return BadRequest($"Failed to delete day off.".ToJson());
            }
            _logger.LogInformation($"Day off on {date} deleted");
            return Ok();
        }

        /// <summary>
        /// The function returns all the AppointmentsAvailableBlock for tenant with id = tenantId
        /// </summary>
        [HttpGet("AvailableBlocks/GetAll")]
        public async Task<ActionResult<List<AppointmentsAvailableBlock>>> GetAvailableBlocks([FromRoute] string tenantId)
        {
            var available = await _appointmentSettingsServices.GetAllAvailableBlocks(tenantId);
            if (available == null)
            {
                _logger.LogError($"Failed to execute GetAllTypes for {tenantId}");
                return BadRequest($"Available blocks list was not found.".ToJson());
            }
            return Ok(available);
        }

        /// <summary>
        /// This endpoint is for creating specific Appointments Available Block in the DB (2 databases) for the tenant with id = tenantId.
        /// </summary>
        [HttpPost("AvailableBlocks/Create")]
        public async Task<IActionResult> CreateNewBlock([FromRoute] string tenantId, [FromBody] AppointmentsAvailableBlockDto availableBlockDto)
        {
            AppointmentsAvailableBlock? availableBlock = availableBlockDto.CreateAvailableBlock();
            if (availableBlock == null)
            {
                _logger.LogError($"Failed to execute CreateAvailableBlock of availableBlock for tenant {tenantId}. Start time is after end time.");
                return BadRequest($"Failed to create available block.".ToJson());
            }

            var validation = await _appointmentSettingsServices.ValidateAvailableBlock(tenantId, availableBlock);
            if (!validation)
            { 
                _logger.LogError($"Failed to execute CreateNewBlock of {availableBlock.ToString()}.Validation failed.");
                return BadRequest($"Failed to create available block.Validation failed.".ToJson());
            }

            var available = await _appointmentSettingsServices.CreateAvailableBlock(tenantId, availableBlock);
            if (available == false)
            {
                _logger.LogError($"Failed to execute CreateNewBlock of {availableBlock.ToString()} for tenant {tenantId}");
                return BadRequest($"Failed to create available block.".ToJson());
            }
            _logger.LogInformation($"New block created in {availableBlock.ToString()} for {tenantId}");
            return Ok(availableBlockDto);
        }

        /// <summary>
        /// This endpoint is for deleting specific Appointments Available Block form DB (2 databses) for the tenant with id = tenantId.
        /// </summary>
        [HttpDelete("AvailableBlocks/Delete")]
        public async Task<IActionResult> DeleteAvailableBlock([FromRoute] string tenantId, [FromBody] AppointmentsAvailableBlockDto availableBlockDto)
        {

            AppointmentsAvailableBlock? availableBlock = availableBlockDto.CreateAvailableBlock();
            if (availableBlock == null)
            {
                _logger.LogError($"Failed to execute DeleteAvailableBlock of availableBlock for tenant {tenantId}. Start time is bigger than end time.");
                return BadRequest($"Failed to delete available block. Start time is bigger than end time.".ToJson());
            }

            var available = await _appointmentSettingsServices.DeleteAvailableBlock(tenantId, availableBlock);
            if (available == false)
            {
                _logger.LogError($"Failed to execute DeleteAvailableBlock of {availableBlock.ToString()} for tenant {tenantId}");
                return BadRequest($"Failed to delete available block.".ToJson());
            }
            _logger.LogInformation($"New block created in {availableBlock.ToString()} for {tenantId}");
            return Ok();
        }

        /// <summary>
        /// The function returns the slot duration for tenant with id = tenantId
        /// </summary>
        [HttpGet("SlotDuration/Get")]
        public async Task<IActionResult> GetSlotDuration([FromRoute] string tenantId)
        {
            var available = await _appointmentSettingsServices.GetSlotDuration(tenantId);
            if (available == null)
            {
                _logger.LogError($"Failed to execute GetAllTypes for {tenantId}");
                return BadRequest($"Slot duration was not found.".ToJson());
            }
            return Ok(available);
        }


        [HttpPut("SlotDuration/Update")]
        public async Task<IActionResult> PutSlotDuration([FromRoute] string tenantId, [FromBody] SlotDurationDto newDuration)
        {

            TimeSpan duration = new TimeSpan(newDuration.Hours, newDuration.Minutes, 0);

            // Check if duration is valid
            var minDuration = Constants.minimumSlotDurationMinutes;
            if (duration.TotalMinutes < minDuration)
            {
                _logger.LogError($"Failed to execute UpdateSlotDuration of {duration} for tenant {tenantId}. Duration is less than {minDuration} minutes.");
                return BadRequest($"Duration is less than {minDuration} minutes.".ToJson());
            }

            var result = await _appointmentSettingsServices.UpdateSlotDuration(tenantId, duration);
            if (result == false)
            {
                _logger.LogError($"Failed to execute UpdateSlotDuration of {duration} for tenant {tenantId}");
                return BadRequest($"Failed to update slot duration.".ToJson());
            }

            if (result == null)
            {
                _logger.LogError($"Failed to execute UpdateSlotDuration of {duration} for tenant {tenantId}. There are available blocks.");
                return BadRequest($"Failed to update slot duration. There are available blocks.".ToJson());
            }

            _logger.LogInformation($"Update slot duration for {tenantId} completed successfully");
            return Ok();
        }


        [HttpPost("GenerateSlots")]
        [Authorize(Policy = Roles.Roles.Tenant)]
        public async Task<ActionResult<Dictionary<DateOnly, List<AppointmentSlot>>>> GenerateSlotsAsync([FromRoute] string tenantId, [FromBody] DateRangeDto dateRangeDto)
        {
            DateOnly start = dateRangeDto.Start.convertToDateOnly();
            DateOnly end = dateRangeDto.End.convertToDateOnly();

            if (end < start)
            {
                _logger.LogError($"Failed to execute GenerateSlotsAsync for {tenantId}");
                return BadRequest($"Failed to generate slots.".ToJson());
            }
            var slots = await _appointmentSettingsServices.GenerateSlots(tenantId,
                new DateTime(start.Year, start.Month, start.Day),
                new DateTime(end.Year, end.Month, end.Day));
            if (slots == null)
            {
                _logger.LogError($"Failed to execute GenerateSlotsAsync for {tenantId}");
                return BadRequest($"Failed to generate slots.".ToJson());
            }

            if (slots.Count == 0)
            {
                _logger.LogInformation($"Generate slots for {tenantId} completed successfully. No slots were generated");
            }
            else
            {
                _logger.LogInformation($"Generate slots for {tenantId} completed successfully. {slots.Count} slots were generated");
            }
            return Ok(slots);
        }
    }
}
