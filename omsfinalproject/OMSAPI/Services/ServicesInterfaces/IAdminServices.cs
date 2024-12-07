using OMSAPI.Dto.EntitiesDto;
using OMSAPI.Models.Entities;
using System.Runtime.CompilerServices;

namespace OMSAPI.Services.ServicesInterfaces
{
    public interface IAdminServices
    {
        Admin CreateAdminFromDto(AdminDto admin, byte[] passwordHash, byte[] passwordSalt);

        Task<Admin?> GetAdminByName(string name);

        Task<string?> Create(Admin admin);
        Task<List<Admin>> GetAll();

        Task DeleteAdmin(Admin admin1);


    }
}
