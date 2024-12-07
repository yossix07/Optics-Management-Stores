using MongoDB.Driver;
using OMSAPI.Models.Entities;

namespace OMSAPI.Services.ServicesInterfaces
{
    public interface IEntityServices
    {
        IMongoCollection<IEntity>? GetCollectionByEntityType(string tenantId, IEntity entity);
        string? GetCollectionNameByEntityType(string tenantId, string role);
        Task<IEntity?> GetEntityByEmail(string tenantId, string email, string collectionName);
    }
}
