using MongoDB.Driver;
using OMSAPI.Dto;
using OMSAPI.Models.Entities;
using Serilog;
using System.Security.Claims;

namespace OMSAPI.Services.ServicesInterfaces
{
    public interface IAuthServices
    {
        string CreateToken<T>(T t, List<Claim> claims);

        bool VerifyPassowordHash(string password, byte[] passwordHash, byte[] passwordSalt);

        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);

        string GenerateVarificationCode();

        Task<bool> SaveVarificationCode(string tenantId, string email, string varificationCode, string role);

        Task<ResetPasswordModel?> GetResetPasswordRequest(string tenantId, string email);

        public IEntity? CreateNewUserPassword(string tenantId, VerifyCodeRequestDto verifyCodeRequest, IEntity entity, string collectionName);

        Task<bool> SaveNewUserPassword(string tenantId, User entity, string collectionName);

        Task<bool> SaveNewTenantPassword(string tenantId, Tenant updatedEntity, string collectionName);
        Task<bool> RemoveResetPasswordRequest(string tenantId, string email);
    }
}
