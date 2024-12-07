using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OMSAPI.Services.ServicesInterfaces;

namespace OMSAPI.Controllers.EntitiesControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Roles.Roles.Admin)]
    public class AdminController : ControllerBase
    {

        private readonly IAdminServices _adminServices;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminServices adminServices, ILogger<AdminController> logger)
        {
            _adminServices = adminServices;
            _logger = logger;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var admins = await _adminServices.GetAll();
            if (admins == null)
            {
                _logger.LogInformation("No admins found");
                return NotFound();
            }
            return Ok(admins);
        }

        [HttpGet("get/{name}")]
        public async Task<IActionResult> GetAdmin([FromRoute] string name)
        {
            var admin = await _adminServices.GetAdminByName(name);
            if (admin == null)
            {
                _logger.LogInformation($"Admin with name {name} not found");
                return NotFound();
            }
            return Ok(admin);
        }

        [HttpDelete("delete/{name}")]
        public async Task<IActionResult> Delete([FromRoute] string name)
        {
            if (name == null)
            {
                _logger.LogInformation("Admin name is null");
                return BadRequest();
            }
            var admin = await _adminServices.GetAdminByName(name);
            if (admin == null)
            {
                _logger.LogInformation($"Admin with name {name} not found");
                return NotFound();
            }
            await _adminServices.DeleteAdmin(admin);
            _logger.LogInformation($"Admin with name {name} deleted successfully");
            return Ok();
        }
    }
}
