using Microsoft.IdentityModel.Tokens;
using OMSAPI.Models.Entities;
using OMSAPI.Services.ServicesInterfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Serilog;
using MongoDB.Driver;
using OMSAPI.Dto;

namespace OMSAPI.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly ILogger<AuthServices> _logger;
        private readonly IDatabaseServices _databaseServices;
        private readonly IConfiguration _configuration;

        public AuthServices(ILogger<AuthServices> logger, IDatabaseServices databaseServices, IConfiguration configuration)
        {
            _logger = logger;
            _databaseServices = databaseServices;
            _configuration = configuration;
        }

        public string CreateToken<T>(T t, List<Claim> claims)
        {
            // Creating JWT token
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AuthSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(General.Constants.tokenMinutesTimeLimit),
                signingCredentials: creds);

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return jwtToken;
        }

        public bool VerifyPassowordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            // using cryptography algorithm 
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                return computedHash.SequenceEqual(passwordHash);
            }
        }

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            // using cryptography algorithm 
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public static bool ValidateAuthorization(string? id, HttpContext context, ClaimsPrincipal claimsPrincipal, string role)
        {
            // Validate id is not null.
            if (id == null) 
            {
                return false;
            }

            // Validate that users ask for their own appointment
            if (claimsPrincipal.IsInRole(role))
            {
                //var authenticatedUserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var authenticatedUserId = context.User?.Identity?.Name;
                if (authenticatedUserId != id)
                {
                    Log.Error($"A User {authenticatedUserId} is trying to create an appointment for another user");
                    return false;
                }
            }
            return true;
        }
       
        public string GenerateVarificationCode()
        {
            // create new random string with 6 characters (letters or digits)
            Random random = new Random();
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 6)
                             .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async Task<bool> SaveVarificationCode(string tenantId, string email, string varificationCode, string role)
        {

            var collection = _databaseServices.FindCollectionByDB<ResetPasswordModel>(tenantId, General.Constants.resetPasswordCollectionName);
            if (collection == null || varificationCode == null)
            {
                return false;
            }

            var resetPasswordModel = new ResetPasswordModel
            {
                Email = email,
                VerificationCode = varificationCode,
                VarificationCodeExpired = DateTime.Now.AddMinutes(General.Constants.verificationCodeExpirationTime),
                Role = role
            };
            if (resetPasswordModel != null)
            {
                try
                {
                    await collection.InsertOneAsync(resetPasswordModel);
                    return true;
                } catch (Exception ex)
                {
                    Log.Error($"Failed to save varification code for {email} with error: {ex.Message}");
                    return false;
                }
            }
            return false;
        }

        public async Task<ResetPasswordModel?> GetResetPasswordRequest(string tenantId, string email)
        {
            var collection = _databaseServices.FindCollectionByDB<ResetPasswordModel>(tenantId, General.Constants.resetPasswordCollectionName);
            if (collection != null)
            {
                try
                {
                    var res = await collection.Find(x => x.Email == email).FirstOrDefaultAsync();
                    if (res != null)
                    {
                        return res;
                    }
                    return null;
                    
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to get varification code for {email} with error: {ex.Message}");
                }
            }
            return null;
        }

        public IEntity? CreateNewUserPassword(string tenantId, VerifyCodeRequestDto verifyCodeRequest, IEntity entity, string collectionName)
        {
            // find collection in DB
            var collection = _databaseServices.FindCollectionByDB<IEntity>(tenantId, collectionName);
            if (collection == null)
            {
                return null;
            }

            // Create password hash and salt for new password
            CreatePasswordHash(verifyCodeRequest.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);

            // update user password
            entity.PasswordHash = passwordHash;
            entity.PasswordSalt = passwordSalt;

            return entity;
        }

        
        public async Task<bool> SaveNewUserPassword(string tenantId, User updatedEntity, string collectionName)
        {
            bool userSaveResult = await SaveNewPassword<User>(tenantId, updatedEntity, General.Constants.usersCollectionName);
            return userSaveResult;
        }

        public async Task<bool> SaveNewTenantPassword(string tenantId, Tenant updatedEntity, string collectionName)
        {
            bool userSaveResult = await SaveNewPassword<Tenant>(tenantId, updatedEntity, General.Constants.detailsCollectionName);
            bool userSaveResultMainDB = await SaveNewPassword<Tenant>(General.Constants.mainDbName, updatedEntity, General.Constants.detailsCollectionName);
            return userSaveResult && userSaveResultMainDB;
        }

        private async Task<bool> SaveNewPassword<T>(string tenantId, T updatedEntity, string collectionName) where T : IEntity
        {
            var collection = _databaseServices.FindCollectionByDB<T>(tenantId, collectionName);
            if (collection == null)
            {
                return false;
            }

            try
            {
                var filter = Builders<T>.Filter.Eq(x => x.Id, updatedEntity.Id);
                var res = await collection.ReplaceOneAsync(filter, updatedEntity);
                if (res != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to update password for {updatedEntity.Email} with error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RemoveResetPasswordRequest(string tenantId, string email)
        {
            var collection = _databaseServices.FindCollectionByDB<ResetPasswordModel>(tenantId, General.Constants.resetPasswordCollectionName);
            if (collection != null)
            {
                try
                {
                    var res = await collection.DeleteOneAsync(x => x.Email == email);
                    if (res != null)
                    {
                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to delete varification code for {email} with error: {ex.Message}");
                }
            }
            return false;
        }

    }

}
