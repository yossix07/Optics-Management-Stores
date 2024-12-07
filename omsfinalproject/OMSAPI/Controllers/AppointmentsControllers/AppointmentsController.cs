using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using OMSAPI.Dto;
using OMSAPI.Dto.AppointmentsDto;
using OMSAPI.Models.Appointments;
using OMSAPI.Services;
using OMSAPI.Services.ServicesInterfaces;

namespace OMSAPI.Controllers.AppointmentsControllers
{

    [ApiController]
    [Route("api/{tenantId}/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly ILogger<AppointmentsController> _logger;
        private IAppointmentServices _appointmentServices;
        private IUserServices _userServices;
        private IAppointmentSettingsServices _appointmentSettingsServices;

        public AppointmentsController(ILogger<AppointmentsController> logger, IAppointmentServices appointmentServices, IUserServices userServices, IAppointmentSettingsServices appointmentSettingsServices)
        {
            _logger = logger;
            _appointmentServices = appointmentServices;
            _userServices = userServices;
            _appointmentSettingsServices = appointmentSettingsServices;
        }


        // GET: Get all appointments for spesific tenant.
        [HttpGet("GetByStatus")]
        [Authorize(Policy = Roles.Roles.Tenant)]
        public async Task<ActionResult<IEnumerable<AppointmentSlot>>> GetAppointmentsAsync([FromRoute] string tenantId, [FromQuery] DateRangeWithStatusDto dateRangeWithStatusDto)
        {
            // Validate that tenant Acess only to his messages.
            if (!AuthServices.ValidateAuthorization(tenantId, HttpContext, User, Roles.Roles.Tenant))
            {
                return Forbid();
            }

            return await GetAppointmentHelper(tenantId, dateRangeWithStatusDto);
        }

        // GET: Get all appointments for spesific tenant.
        [HttpGet("GetAvailable")]
        [Authorize(Policy = Roles.Roles.User)]
        public async Task<ActionResult<IEnumerable<AppointmentSlot>>> GetAvailableAppointmentsSlotsAsync([FromRoute] string tenantId, [FromQuery] DateRangeDto dateRangeDto)
        {

            // Validate that tenant Acess only to his messages.
            if (!AuthServices.ValidateAuthorization(HttpContext.User?.Identity?.Name, HttpContext, User, Roles.Roles.User))
            {
                return Forbid();
            }

            DateRangeWithStatusDto dateRangeWithStatusDto = dateRangeDto.CreateRangeWithAvailableStatus();
            return await GetAppointmentHelper(tenantId, dateRangeWithStatusDto);

        }

        /// <summary>
        /// This is a helper function, this function helps to get the appointments by status. Needed because there are 2 function with different authorities.
        /// User can access only to available slots.
        /// </summary>
        [NonAction]
        public async Task<ActionResult<IEnumerable<AppointmentSlot>>> GetAppointmentHelper([FromRoute] string tenantId, [FromQuery] DateRangeWithStatusDto dateRangeWithStatusDto)
        {
            // Conver dateRangeDto to Dateonly objectes.
            try
            {
                DateOnly start = dateRangeWithStatusDto.Start.convertToDateOnly();
                DateOnly end = dateRangeWithStatusDto.End.convertToDateOnly();

                var users = await _userServices.GetAll(tenantId);
                if (users == null)
                {
                    _logger.LogError($"Faild to retrun appointements for {tenantId}. Failed to get users.");
                    return BadRequest("Users was not found.".ToJson());
                }

                var appointments = await _appointmentServices.GetAppointmentsByDateAndStatus(tenantId, start, end, dateRangeWithStatusDto.Status.ToString(), users);

                if (appointments == null)
                {
                    _logger.LogError($"Faild to retrun appointements for {tenantId}");
                    return BadRequest("Failed to get appointments.".ToJson());
                }

                appointments = appointments.Value?.Where(x => x.Key >= DateOnly.FromDateTime(DateTime.Today))
                                       .ToDictionary(x => x.Key, x => x.Value) ?? new Dictionary<DateOnly, List<AppointmentSlotDto>>();
                return Ok(appointments);
            }
            catch
            {
                return General.Utils.LogErrorAndReturnBadRequest(_logger, $"Invalid date format.".ToJson());
            }
        }


        //GET: GetById all customer appointments for spesific tenant
        [HttpGet("{userId}")]
        [Authorize(Policy = Roles.Roles.User)]
        public async Task<ActionResult<Dictionary<DateOnly, List<AppointmentSlotDto>>>?> GetAllUserAppointmentsAsync([FromRoute] string tenantId, [FromRoute] string userId)
        {
            // Verify Authorize user or tenant.
            if (!AuthServices.ValidateAuthorization(userId, HttpContext, User, Roles.Roles.User) ||
                !AuthServices.ValidateAuthorization(tenantId, HttpContext, User, Roles.Roles.Tenant))
            {
                return Forbid();
            }

            // Get and Validate user
            var user = await _userServices.GetById(tenantId, userId);
            if (user == null)
            {
                _logger.LogError($"Faild to retrun appointements for {userId}. User Does not found");
                return BadRequest("User was not found.".ToJson());
            }

            var appointments = await _appointmentServices.GetAllUserAppointments(tenantId, user);

            if (appointments == null)
            {
                _logger.LogError($"Faild to retrun appointements for {tenantId}");
                return BadRequest("Failed to get appointments.".ToJson());
            }

            // return only appointments keys date is bigger or equal to today.
            appointments = appointments.Value?.Where(x => x.Key >= DateOnly.FromDateTime(DateTime.Today))
                       .ToDictionary(x => x.Key, x => x.Value) ?? new Dictionary<DateOnly, List<AppointmentSlotDto>>();
            return Ok(appointments);
        }


        // GET: Get appointment with Id - 
        [HttpGet("get/{appointmentId}")]
        [Authorize(Policy = Roles.Roles.User)]
        public async Task<ActionResult<Dictionary<DateOnly, AppointmentSlotDto>>> GetAppointmentByIdAsync([FromRoute] string tenantId, [FromRoute] string appointmentId)
        {
            var appointments = await _appointmentServices.GetAppointmentById(tenantId, appointmentId);

            if (appointments == null)
            {
                _logger.LogError($"Faild to retrun appointements for {tenantId}");
                return BadRequest($"Failed to get appointments".ToJson());
            }

            return Ok(appointments);
        }


        // POST: create mew appointment.
        [HttpPost]
        [Authorize(Policy = Roles.Roles.User)]
        public async Task<ActionResult<AppointmentSlot>> CreateNewAppointmentAsync([FromRoute] string tenantId, [FromBody] CreateAppointmentDto appointmentCreateDto)
        {
            if (!AuthServices.ValidateAuthorization(appointmentCreateDto.UserId, HttpContext, User, Roles.Roles.User) ||
                !AuthServices.ValidateAuthorization(tenantId, HttpContext, User, Roles.Roles.Tenant))
            {
                return Forbid();
            }

            // create new appointment in DB
            var created = await _appointmentServices.CreateAppointment(tenantId, appointmentCreateDto);
            if (created)
            {
                return Ok($"Appointment created for user {appointmentCreateDto.UserId}");
            }

            return General.Utils.LogErrorAndReturnBadRequest(_logger, $"Faild to create appointment {appointmentCreateDto.AppointmentId}");
        }

        // DELETE: delete appointment with id = appointmentId
        [HttpDelete("{appointmentId}")]
        [Authorize(Policy = Roles.Roles.User)]
        public async Task<IActionResult> DeleteAppointmentAsync([FromRoute] string tenantId, [FromRoute] string appointmentId, [FromBody] string userId)
        {
            if (!AuthServices.ValidateAuthorization(userId, HttpContext, User, Roles.Roles.User) ||
                !AuthServices.ValidateAuthorization(tenantId, HttpContext, User, Roles.Roles.Tenant))
            {
                return Forbid();
            }

            // Delete appointment from DB.
            var deleted = await _appointmentServices.DeleteAppointment(tenantId, appointmentId);
            if (deleted)
            {
                _logger.LogInformation($"Appointment {appointmentId} deleted.");
                return Ok();
            }
            return General.Utils.LogErrorAndReturnBadRequest(_logger, $"Faild to delete appointment {appointmentId}");
        }


        [HttpPost("CreateCustomAppointment")]
        [Authorize(Policy = Roles.Roles.Tenant)]
        public async Task<ActionResult<AppointmentSlot>> CreateCustomAppointmentSlotAsync([FromRoute] string tenantId, [FromBody] CreateCustomAppointmentDto createCustomDto)
        {
            if (!AuthServices.ValidateAuthorization(tenantId, HttpContext, User, Roles.Roles.Tenant))
            {
                return Forbid();
            }

            try
            {
                var date = createCustomDto.Date.ConvertToDateTime();
                var day = date.DayOfWeek;
                AppointmentsAvailableBlock? customBlock = createCustomDto.AppointmentsAvailableBlockDto.CreateAvailableBlock();
                // create slot duration for custom appointment by substract end and start time
                if (customBlock != null)
                {
                    var slotDuration = customBlock.EndTime - customBlock.StartTime;
                    var slot = await _appointmentSettingsServices.GenerateSlots(tenantId, date, date, customBlock, slotDuration);
                    // check if slot is not null or empty
                    if (slot != null)
                    {
                        // get list from slot dictionary on date key.
                        var dateonly = createCustomDto.Date.convertToDateOnly();
                        if (slot[dateonly].Count > 0)
                        {
                            _logger.LogInformation($"Custom slot created");
                            return Ok(slot);
                        }
                        // if the list is empty, return bad request because the slot was not created.
                        else
                        {
                            return General.Utils.LogErrorAndReturnBadRequest(_logger, $"Faild to create custom appointment.".ToJson());
                        }
                    }
                }
            }
            catch
            {
                return General.Utils.LogErrorAndReturnBadRequest(_logger, $"Faild to create custom appointment.".ToJson());
            }

            return General.Utils.LogErrorAndReturnBadRequest(_logger, $"Faild to create custom appointment.".ToJson());

        }
    }
}
