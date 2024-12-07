using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OMSAPI.Models.Entities;
using OMSAPI.Services.ServicesInterfaces;
using OMSAPI.Services;
using OMSAPI.Dto.EntitiesDto;
using MongoDB.Bson;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OMSAPI.Controllers.EntitiesControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantController : ControllerBase
    {
        private readonly ITenantServices _tenantService;
        private readonly ILogger<TenantController> _logger;
        public TenantController(ITenantServices tenantService, ILogger<TenantController> logger)
        {
            _tenantService = tenantService;
            _logger = logger;
        }

        // GET: api/<TenantController>
        [HttpGet]
        [Authorize(Policy = Roles.Roles.Admin)]
        public async Task<ActionResult<List<Tenant>?>> GetAll()
        {
            var list = await _tenantService.GetAll();
            if (list == null)
            {
                return General.Utils.LogErrorAndReturnBadRequest(_logger, "Get all tenants operation failed".ToJson());
            }
            _logger.LogInformation($"GetAll operation finished successfully");
            return Ok(list);
        }

        // GET api/<TenantController>/5
        [HttpGet("{id}")]
        [Authorize(Policy = Roles.Roles.User)]
        public async Task<ActionResult<Tenant>> Get(string id)
        {

            // Validate that tenant Acess only to his messages.
            if (!AuthServices.ValidateAuthorization(id, HttpContext, User, Roles.Roles.Tenant) ||
                !AuthServices.ValidateAuthorization(HttpContext.User?.Identity?.Name, HttpContext, User, Roles.Roles.User))
            {
                return Forbid();
            }

            var tenant = await _tenantService.Get(id);

            if (tenant == null)
            {
                _logger.LogError($"Failed to execute Get for {id}. Tenant was not found.");
                return NotFound($"Tenant with Id {id} was not found".ToJson());
            }
            _logger.LogInformation($"Get operation for Tenant with id = {id} finished successfully");
            return Ok(tenant);
        }


        // PUT api/<TenantController>/5
        [HttpPut("{id}")]
        [Authorize(Policy = Roles.Roles.Tenant)]
        public async Task<IActionResult> Put(string id, [FromBody] UpdateTenantDto tenant)
        {
            // Validate that tenant Acess only to his messages.
            if (!AuthServices.ValidateAuthorization(id, HttpContext, User, Roles.Roles.Tenant))
            {
                return Forbid();
            }

            var existingTenant = await _tenantService.Get(id);

            if (existingTenant == null)
            {
                _logger.LogError($"Failed to execute Put function for {id}. Tenant was not found.");
                return NotFound($"Tenant with Id {id} was not found".ToJson());
            }

            var res = await _tenantService.Update(id, tenant);

            if (res == null)
            {
                _logger.LogError($"Failed to execute Put for {id}. Update failed.");
                return NotFound($"Error! Update for {id} failed".ToJson());
            }

            _logger.LogInformation($"Tenant with id = {id} updated successfully");
            return Ok();
        }

        // DELETE api/<TenantController>/5
        [HttpDelete("{id}")]
        [Authorize(Policy = Roles.Roles.Admin)]
        public async Task<IActionResult> Delete(string id)
        {
            var existingTenant = await _tenantService.Get(id);

            if (existingTenant == null)
            {
                _logger.LogError($"Unable to delete tenant: {id}");
                return NotFound($"Unable to delete tenant: {id}".ToJson());
            }

            var res = await _tenantService.Delete(id);
            if (res == null)
            {
                _logger.LogError($"Unable to delete {id}");
                return BadRequest($"Unable to delete {id}".ToJson());
            }

            _logger.LogInformation($"{existingTenant.Id} deleted");
            return Ok($"Tenant with Id = {id} deleted");
        }
    }
}
