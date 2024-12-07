using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using OMSAPI.Dto.EntitiesDto;
using OMSAPI.Models.Entities;
using OMSAPI.Services;
using OMSAPI.Services.ServicesInterfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OMSAPI.Controllers.EntitiesControllers
{
    [Route("api/{tenantId}/[controller]")]
    [ApiController]
    [Authorize(Policy = Roles.Roles.User)]
    public class UserController : ControllerBase
    {
        private IUserServices _userService;
        private readonly ILogger<UserController> _logger;
        public UserController(IUserServices userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = Roles.Roles.Tenant)]
        public async Task<ActionResult<List<User>>> GetAll([FromRoute] string tenantId)
        {
            // Validate that tenant Acess only to his messages.
            if (!AuthServices.ValidateAuthorization(tenantId, HttpContext, User, Roles.Roles.Tenant))
            {
                return Forbid();
            }

            var list = await _userService.GetAll(tenantId);

            if (list == null)
            {
                _logger.LogError($"Failed to execute GetAll for {tenantId}");
                return BadRequest("Database or collection was not found".ToJson());
            }
            _logger.LogInformation($"GetAll operation finished successfully");
            return Ok(list);
        }


        [HttpGet("{userId}")]
        public async Task<ActionResult<User>> Get([FromRoute] string tenantId, [FromRoute] string userId)
        {
            // Validate that tenant Acess only to his messages.
            if (!AuthServices.ValidateAuthorization(userId, HttpContext, User, Roles.Roles.User) ||
                !AuthServices.ValidateAuthorization(tenantId, HttpContext, User, Roles.Roles.Tenant))
            {
                return Forbid();
            }

            var user = await _userService.GetById(tenantId, userId);

            if (user == null)
            {
                _logger.LogError($"GetById opration failed for user {userId} in {tenantId} database.");
                return NotFound($"User with Id {userId} was not found");
            }
            _logger.LogInformation($"GetById operation finished successfully".ToJson());
            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] string tenantId, string id, [FromBody] UpdateUserDto user)
        {
            // Validate that tenant Acess only to his messages.
            if (!AuthServices.ValidateAuthorization(HttpContext.User?.Identity?.Name, HttpContext, User, Roles.Roles.User) ||
                !AuthServices.ValidateAuthorization(tenantId, HttpContext, User, Roles.Roles.Tenant))
            {
                return Forbid();
            }

            var existingUser = await _userService.GetById(tenantId, id);

            if (existingUser == null)
            {
                _logger.LogError($"Failed to execute Put for {id}. Update failed.");
                return BadRequest($"User with Id {id} was not found");
            }

            var res = await _userService.Update(tenantId, id, user);
            if (res == null)
            {
                _logger.LogError($"Failed to execute Put for {id}. Update failed.");
                return NotFound($"Error! Update for {id} failed");
            }

            _logger.LogInformation($"Put operation finished successfully");
            return Ok();

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string tenantId, string id)
        {
            // Validate that tenant Acess only to his messages.
            if (!AuthServices.ValidateAuthorization(id, HttpContext, User, Roles.Roles.User) ||
                !AuthServices.ValidateAuthorization(tenantId, HttpContext, User, Roles.Roles.Tenant))
            {
                return Forbid();
            }

            var existingUser = await _userService.GetById(tenantId, id);

            if (existingUser == null)
            {
                _logger.LogError($"Failed to execute Delte for {id}. Delete failed.");
                return NotFound($"User with Id {id} was not found");
            }

            var res = await _userService.Delete(tenantId, id);
            if (res == null)
            {
                _logger.LogError($"Failed to execute Delete for {id}. Update failed.");
                return NotFound($"Error! Update for {id} failed");
            }

            return Ok();
        }

    }
}
