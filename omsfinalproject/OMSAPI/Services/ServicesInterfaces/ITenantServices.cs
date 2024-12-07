using Microsoft.AspNetCore.Mvc;
using OMSAPI.Dto.EntitiesDto;
using OMSAPI.Models.Entities;

namespace OMSAPI.Services.ServicesInterfaces
{

    public interface ITenantServices
    {
        Task<List<Tenant>?> GetAll();

        Task<Tenant?> Get(string id);

        Task<Tenant?> GetTenantByName(string name);

        Task<Tenant?> GetTenantByEmail(string email);

        Task<string?> Create(Tenant tenant);

        Task<Tenant?> Update(string id, UpdateTenantDto tenant);

        Task<String?> Delete(string id);

        Tenant CreateTenantFromDto(TenantDto tenant, byte[] passwordHash, byte[] passwordSalt);

        //Task<Tenant?> UpdateTenantFromDto(TenantDto tenant, byte[] passwordHash, byte[] passwordSalt)
    }
}
