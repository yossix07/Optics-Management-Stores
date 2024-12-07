using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using OMSAPI.Services.ServicesInterfaces;
using OMSAPI.Models.Entities;
using OMSAPI.Dto.EntitiesDto;
using OMSAPI.Dto.AuthDto;
using OMSAPI.Dto;
using OMSAPI.Services.EntitiesServices;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Bson;

namespace OMSAPI.Controllers.AuthenticationControllers
{
    //TODO:: Need to add Authorization for this class - Who can create Tenants?
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthServices _authServices;
        private readonly IDatabaseServices _databaseServices;
        private readonly ITenantServices _tenantServices;
        private readonly IUserServices _userServices;
        private readonly AdminServices _adminServices;
        private readonly ILogger<AuthController> _logger;
        private readonly IEntityServices _entityServices;
        private readonly IEmailServices _emailServices;

        public AuthController(IConfiguration configuration, IAuthServices authServices, IDatabaseServices databaseServices,
            ITenantServices tenantServices, IUserServices userServices, AdminServices adminServices, ILogger<AuthController> logger, IEntityServices entityServices, IEmailServices emailServices)
        {
            _configuration = configuration;
            _authServices = authServices;
            _databaseServices = databaseServices;
            _tenantServices = tenantServices;
            _userServices = userServices;
            _adminServices = adminServices;
            _logger = logger;
            _entityServices = entityServices;
            _emailServices = emailServices;

        }

        [HttpPost("register/{tenantId}/user")]
        public async Task<ActionResult<User>> RegisterUser([FromRoute] string tenantId, UserDto request)
        {
            _authServices.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var collection = _databaseServices.FindCollectionByDB<User>(tenantId, "users");
            if (collection == null)
            {
                return BadRequest("Could not find users collection");
            }

            User user = _userServices.CreateUserFromDto(request, passwordHash, passwordSalt);

            var error = await _userServices.Create(tenantId, user);
            if (error == null)
            {
                return Ok(user);
            }
            _logger.LogError($"Faild to create user {user.Id} for tenant {tenantId}, {error} unique key error.");
            return BadRequest(error);

        }

        [HttpPost("login/{tenantId}/user")]
        public async Task<ActionResult<string>> LoginUser([FromRoute] string tenantId, [FromBody] LoginUserDto request)
        {

            User? user = await _userServices.GetById(tenantId, request.Id);

            if (user == null)
            {
                _logger.LogError($"Login user failed for {request.Id}. User was not found");
                return BadRequest("User not found");
            }

            // Verify passowrd
            if (!_authServices.VerifyPassowordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                _logger.LogError($"Login user failed for {request.Id}. Wrong password");
                return BadRequest("Wrong Password");
            }

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id),
                new Claim(ClaimTypes.Role, Roles.Roles.User.ToString())
            };

            string token = _authServices.CreateToken(user, claims);
            _logger.LogInformation($"Login for {request.Id} finished successfully.");
            return Ok(token);
        }



        [HttpPost("register/tenant")]
        [Authorize(Policy = Roles.Roles.Admin)]
        public async Task<ActionResult<Tenant>> RegisterTenant(TenantDto request)
        {

            _authServices.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var collection = _databaseServices.FindCollectionByDB<Tenant>(General.Constants.mainDbName, General.Constants.mainDbTenantCollectionName);
            if (collection == null)
            {
                return BadRequest("Could not find tenants collection");
            }

            Tenant tenant = _tenantServices.CreateTenantFromDto(request, passwordHash, passwordSalt);

            var error = await _tenantServices.Create(tenant);
            if (error == null)
            {
                _logger.LogInformation($"Register for {request.Name} finished successfully.");
                return Ok(tenant);
            }

            return BadRequest($"Faild to create tenant, {error} unique key error.");
        }

        [HttpPost("login/tenant")]
        public async Task<ActionResult<string>> LoginTenant(LoginTenantDto request)
        {

            Tenant? tenant = await _tenantServices.GetTenantByName(request.Name);

            // Verify the tenant is exists.
            if (tenant == null)
            {
                _logger.LogError($"Login tenant failed for {request.Name}. Tenant was not found");
                return BadRequest("Wrong Username or Password".ToJson());
            }

            if (request.tenantId != tenant.Id)
            {
                _logger.LogError($"Login tenant failed for {request.Name}. Tenant id is not correct");
                return BadRequest("Invalid Input");
            }

            // Verify passowrd.
            if (!_authServices.VerifyPassowordHash(request.Password, tenant.PasswordHash, tenant.PasswordSalt))
            {
                _logger.LogError($"Login tenant failed for {request.Name}. Wrong password");
                return BadRequest("Wrong Username or Password");
            }

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, tenant.Id),
                new Claim(ClaimTypes.Role, Roles.Roles.Tenant.ToString())
            };

            string token = _authServices.CreateToken(tenant, claims);
            _logger.LogInformation($"Login for {request.Name} finished successfully.");
            return Ok(token);
        }


        [HttpPost("register/admin")]
        [Authorize(Policy = Roles.Roles.Admin)]
        public async Task<ActionResult<Tenant>> RegisterAdmin(AdminDto request)
        {

            _authServices.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var collection = _databaseServices.FindCollectionByDB<Tenant>(General.Constants.mainDbName, General.Constants.mainDbTenantCollectionName);
            if (collection == null)
            {
                return BadRequest("Could not find tenants collection");
            }

            Admin admin = _adminServices.CreateAdminFromDto(request, passwordHash, passwordSalt);

            var error = await _adminServices.Create(admin);
            if (error == null)
            {
                _logger.LogInformation($"Register for {request.Name} finished successfully.");
                return Ok(admin);
            }

            return BadRequest($"Faild to create tenant, {error} unique key error.");
        }

        [HttpPost("login/admin")]
        public async Task<ActionResult<string>> LoginAdmin(LoginAdminDto request)
        {

            Admin? admin = await _adminServices.GetAdminByName(request.Name);

            // Verify the tenant is exists.
            if (admin == null)
            {
                _logger.LogError($"Login admin failed for {request.Name}. Admin was not found");
                return BadRequest("Admin not found");
            }

            // Verify passowrd.
            if (!_authServices.VerifyPassowordHash(request.Password, admin.PasswordHash, admin.PasswordSalt))
            {
                _logger.LogError($"Login admin failed for {request.Name}. Wrong password");
                return BadRequest("Wrong Password");
            }

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, admin.Id),
                new Claim(ClaimTypes.Role, Roles.Roles.Admin.ToString())
            };

            string token = _authServices.CreateToken(admin, claims);
            _logger.LogInformation($"Login for {request.Name} finished successfully.");
            return Ok(token);
        }


        [HttpPost("resetPassword/{tenantId}")]
        public async Task<ActionResult> ResetPassword([FromRoute] string tenantId, ResetPasswordDto resetPasswordDto)
        {
            if (resetPasswordDto.Email == null)
            {
                return General.Utils.LogErrorAndReturnBadRequest(_logger, "Failed to reset password.");
            }

            // Get collection name by entity type
            var collectionName = _entityServices.GetCollectionNameByEntityType(tenantId, resetPasswordDto.Role);
            if (collectionName == null)
            {
                return General.Utils.LogErrorAndReturnBadRequest(_logger, "Failed to reset password.");
            }

            // check if role is user or tenant
            dynamic? entity = null;
            if (resetPasswordDto.Role.ToLower() == Roles.Roles.User.ToString().ToLower())
            {
                entity = await _userServices.GetUserByEmail(tenantId, resetPasswordDto.Email);
            }
            else if (resetPasswordDto.Role.ToLower() == Roles.Roles.Tenant.ToString().ToLower())
            {
                entity = await _tenantServices.GetTenantByEmail(resetPasswordDto.Email);
            }

            // Check if entity is null
            if (entity == null)
            {
                return General.Utils.LogErrorAndReturnBadRequest(_logger, "Failed to reset password.");
            }

            // Generate varification code and save it in DB
            var varificationCode = _authServices.GenerateVarificationCode();
            var result = await _authServices.SaveVarificationCode(tenantId, entity.Email, varificationCode, resetPasswordDto.Role);
            if (result != false)
            {
                result = _emailServices.SendResetPasswordEmail(entity.Email, varificationCode);
                if (result != false)
                {
                    return Ok("Verification Email has been sent to you.Please check your email.");
                }
            }
            return General.Utils.LogErrorAndReturnBadRequest(_logger, "Failed to reset password.");
        }

        [HttpPost("verifyCode/{tenantId}")]
        public async Task<ActionResult> ForgetPassword([FromRoute] string tenantId, VerifyCodeRequestDto verifyCodeRequest)
        {
            var resetPasswordRequest = await _authServices.GetResetPasswordRequest(tenantId, verifyCodeRequest.Email);

            if (resetPasswordRequest != null)
            {
                if (resetPasswordRequest.VarificationCodeExpired > DateTime.UtcNow && resetPasswordRequest.VerificationCode == verifyCodeRequest.VerificationCode)
                {
                    if (resetPasswordRequest.Role.ToLower() == Roles.Roles.User.ToString().ToLower())
                    {
                        return await ForgetUserPassword(tenantId, verifyCodeRequest);
                    }
                    else if (resetPasswordRequest.Role.ToLower() == Roles.Roles.Tenant.ToString().ToLower())
                    {
                        return await ForgetTenantPassword(tenantId, verifyCodeRequest);
                    }
                }
            }
            return General.Utils.LogErrorAndReturnBadRequest(_logger, "Failed to reset password.");
        }


        private async Task<ActionResult> ForgetUserPassword(string tenantId, VerifyCodeRequestDto verifyCodeRequest)
        {
            // Get user by email
            var user = await _userServices.GetUserByEmail(tenantId, verifyCodeRequest.Email);
            if (user != null)
            {
                var updatedUser = _authServices.CreateNewUserPassword(tenantId, verifyCodeRequest, user, General.Constants.usersCollectionName);
                if (updatedUser != null)
                {
                    user.PasswordSalt = updatedUser.PasswordSalt;
                    user.PasswordHash = updatedUser.PasswordHash;
                    var result = await _authServices.SaveNewUserPassword(tenantId, user, General.Constants.usersCollectionName);
                    if (result)
                    {
                        await _authServices.RemoveResetPasswordRequest(tenantId, user.Email);
                        return Ok("Verification code is correct. New password was saved successfully");
                    }
                }
            }
            return General.Utils.LogErrorAndReturnBadRequest(_logger, "Failed to reset password.");

        }

        private async Task<ActionResult> ForgetTenantPassword(string tenantId, VerifyCodeRequestDto verifyCodeRequest)
        {
            // Get tenant by email
            var tenant = await _tenantServices.GetTenantByEmail(verifyCodeRequest.Email);
            if (tenant != null)
            {
                var updatedUser = _authServices.CreateNewUserPassword(tenantId, verifyCodeRequest, tenant, General.Constants.detailsCollectionName);
                if (updatedUser != null)
                {
                    tenant.PasswordSalt = updatedUser.PasswordSalt;
                    tenant.PasswordHash = updatedUser.PasswordHash;
                    var result = await _authServices.SaveNewTenantPassword(tenantId, tenant, General.Constants.detailsCollectionName);
                    if (result)
                    {
                        await _authServices.RemoveResetPasswordRequest(tenantId, tenant.Email);
                        return Ok("Verification code is correct. New password was saved successfully");
                    }
                }
            }
            return General.Utils.LogErrorAndReturnBadRequest(_logger, "Failed to reset password.");
        }
    }
}
