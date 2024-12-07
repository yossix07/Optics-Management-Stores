using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OMSAPI.DatabaseSettings;
using OMSAPI.Models.Entities;
using OMSAPI.Services.ServicesInterfaces;

namespace OMSAPI.Services.EntitiesServices
{
    public class EntityServices : IEntityServices
    {
        private readonly IMongoClient _mongoClient;
        private readonly IDatabaseServices _databaseServices;
        private readonly ILogger<EntityServices> _logger;

        public EntityServices(IOptions<AdminDatabaseSettings> databaseSettings, IDatabaseServices databaseServices, ILogger<EntityServices> logger)
        {
            _mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
            _databaseServices = databaseServices;
            _logger = logger;
        }

        public string? GetCollectionNameByEntityType(string tenantId, string role)
        {
            switch (role.ToLower())
            {
                case "user":
                    return General.Constants.usersCollectionName;
                case "tenant":
                    return General.Constants.detailsCollectionName;
                default:
                    return null;
            }

        }

        public async Task<IEntity?> GetEntityByEmail(string tenantId, string email, string collectionName)
        {
            var collection = _databaseServices.FindCollectionByDB<IEntity>(tenantId, collectionName);
            if (collection == null)
            {
                _logger.LogError($"The collection {collectionName} was not fould in the database {tenantId}");
                return null;
            }
            // show all collection as a list
            var res = await collection.Find(entity => true).ToListAsync();
            // var res = await collection.Find(entity => entity.Email == email).FirstOrDefaultAsync();
            return res.First();
        }

        public IMongoCollection<IEntity>? GetCollectionByEntityType(string tenantId, Type entityType)
        {
            if (entityType == typeof(User))
            {
                return _databaseServices.FindCollectionByDB<User>(tenantId, General.Constants.usersCollectionName) as IMongoCollection<IEntity>;
            }
            else if (entityType == typeof(Tenant))
            {
                return _databaseServices.FindCollectionByDB<Tenant>(tenantId, General.Constants.detailsCollectionName) as IMongoCollection<IEntity>;
            }
            else
            {
                // Handle other cases if needed
                return null;
            }
        }

        public IMongoCollection<IEntity>? GetCollectionByEntityType(string tenantId, IEntity entity)
        {
            if (entity is User)
            {
                return _databaseServices.FindCollectionByDB<User>(tenantId, General.Constants.usersCollectionName) as IMongoCollection<IEntity>;
            }
            else if (entity is Tenant)
            {
                return _databaseServices.FindCollectionByDB<Tenant>(tenantId, General.Constants.detailsCollectionName) as IMongoCollection<IEntity>;
            }
            else
            {
                // Handle other cases if needed
                return null;
            }
        }
    }
}
