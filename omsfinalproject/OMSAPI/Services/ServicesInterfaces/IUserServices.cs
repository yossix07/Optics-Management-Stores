using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OMSAPI.Dto.EntitiesDto;
using OMSAPI.Models.Entities;

namespace OMSAPI.Services.ServicesInterfaces
{
    public interface IUserServices
    {
        Task<List<User>?> GetAll(string dbName);

        Task<User?> GetById(string dbName, string userId);

        Task<User?> GetUserByEmail(string dbName, string email);

        Task<string?> Create(string dbName, User user);

        Task<User?> Update(string dbName, string id, UpdateUserDto updatedUser);

        Task<String?> Delete(string dbName, string id);

        User CreateUserFromDto(UserDto userDto, byte[] passwordHash, byte[] passwordSalt);
    }
}
