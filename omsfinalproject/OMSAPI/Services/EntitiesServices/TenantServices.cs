using MongoDB.Driver;
using MongoDB.Bson;
using OMSAPI.AccessDatabase;
using OMSAPI.Services.ServicesInterfaces;
using OMSAPI.Models.Appointments;
using OMSAPI.Models.Entities;
using OMSAPI.Dto.EntitiesDto;
using OMSAPI.Models.Store;

namespace OMSAPI.Services.EntitiesServices
{
    public class TenantServices : ITenantServices
    {
        private readonly IMongoCollection<Tenant> _tenantCollection;
        private readonly IMongoClient _mongoClient;
        private readonly ILogger<TenantServices> _logger;
        private readonly IDatabaseServices _databaseServices;


        public TenantServices(IDatabaseSettings settings, IMongoClient mongoClient, ILogger<TenantServices> logger, IDatabaseServices databaseServices)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _tenantCollection = database.GetCollection<Tenant>(settings.CollectionName);
            _mongoClient = mongoClient;
            _logger = logger;
            _databaseServices = databaseServices;
        }


        public async Task<string?> Create(Tenant tenant)
        {
            try
            {
                // Create new tenant in collection.
                await _tenantCollection.InsertOneAsync(tenant);
            }
            catch (MongoWriteException ex)
            {
                if (ex.Message.Contains("name_1"))
                {
                    _logger.LogError($"{tenant.Name} already exists in the system");
                    return tenant.Name;
                }
                else if (ex.Message.Contains("email_1"))
                {
                    _logger.LogError($"{tenant.Email} already exists in the system");
                    return tenant.Email;
                }
            }
            catch
            {
                _logger.LogError($"Faild to create tenant {tenant.Name}");
                return "Failed";
            }

            // Creating a new Database named - tenantId
            CreateNewDatabase(tenant.Id, tenant);
            return null;
        }

        private async void CreateNewDatabase(string dbName, Tenant tenant)
        {
            try
            {
                // create new database 
                var databaseName = $"{dbName}";
                var database = _mongoClient.GetDatabase(databaseName);

                // create new Details collection.
                // var collectionName = General.Constants.detailsCollectionName;
                // var tenantCollection = database.GetCollection<Tenant>(collectionName);

                // Create new collections - each tenant database will initialize with those collections.
                var tenantDetailsCollection = CreateDetailsCollection(dbName);
                var usersCollection = CreateUsersCollection(dbName);
                var productsCollection = CreateProductsCollection(dbName);
                var ordersCollection = CreateOrdersCollection(dbName);
                var appointmentsCollection = CreateAppointmentsCollection(dbName);

                // Adding the tenant details into details collection
                await tenantDetailsCollection.InsertOneAsync(tenant);
            }
            catch
            {
                _logger.LogError($"Failed to create database");
                return;
            }

            _logger.LogInformation($"Database {dbName} created successfully");
            return;
        }


        public async Task<string?> Delete(string id)
        {
            // Checking if database is exists in the mongo client.
            if (_mongoClient.ListDatabaseNames().ToList().Contains(id))
            {
                var database = _mongoClient.GetDatabase(id);
                await _mongoClient.DropDatabaseAsync(id);
                _logger.LogInformation($"Database {id} dropped");
            }
            else
            {
                _logger.LogError($"Connot find database named {id}");
                return null;
            }
            await _tenantCollection.DeleteOneAsync(tenant => tenant.Id == id);
            _logger.LogInformation($"Tenant {id} removed for tenants collection");
            return id;
        }


        public async Task<Tenant?> Get(string id)
        {
            return await _tenantCollection.Find(tenant => tenant.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Tenant?> GetTenantByName(string name)
        {
            return await _tenantCollection.Find(tenant => tenant.Name == name).FirstOrDefaultAsync();
        }

        public async Task<Tenant?> GetTenantByEmail(string email)
        {
            return await _tenantCollection.Find(tenant => tenant.Email == email).FirstOrDefaultAsync();
        }

        public async Task<List<Tenant>?> GetAll()
        {
            return await _tenantCollection.Find(tenant => true).ToListAsync();
        }

        public async Task<Tenant?> Update(string id, UpdateTenantDto tenant)
        {
            //find the Tenant in the DB
            Tenant? oldTenant = await Get(id);

            if (oldTenant == null)
            {
                return null;
            }

            Tenant? newTenant = CreateTenantFromUpdateDto(oldTenant, tenant);

            if (newTenant == null)
            {
                _logger.LogError($"Update for id {id} failed");
                return null;
            }

            newTenant.Id = oldTenant.Id;
            await _tenantCollection.ReplaceOneAsync(t => t.Id == id, newTenant);
            _logger.LogInformation($"Tenant {newTenant.Name} updated");

            // Update details collection in Tenant DB 
            if (await UpdaetDetailsCollection(newTenant, id))
            {
                return newTenant;
            }

            _logger.LogError($"Update for id {id} failed, cannot find tenant details");
            return null;

        }

        private async Task<bool> UpdaetDetailsCollection(Tenant newTenant, string id)
        {
            // Find the details collection.
            var tenantDetailsCollection = _databaseServices.FindCollectionByDB<Tenant>(newTenant.Id, "details");

            if (tenantDetailsCollection != null)
            {
                // Update the details values.
                await tenantDetailsCollection.ReplaceOneAsync(t => t.Id == id, newTenant);
                return true;
            }
            return false;

        }

        public Tenant CreateTenantFromDto(TenantDto tenant, byte[] passwordHash, byte[] passwordSalt)
        {

            return new Tenant()
            {
                Name = tenant.Name,
                Email = tenant.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                appointmentSettings = AppointmentSettings.CreateDefaultInstace(),
                PhoneNumber = tenant.PhoneNumber,
                Address = tenant.Address,
            };
        }

        public Tenant CreateTenantFromUpdateDto(Tenant tenant, UpdateTenantDto updatedTenant)
        {
            var name = updatedTenant.Name ?? tenant.Name;
            var email = updatedTenant.Email ?? tenant.Email;
            var phoneNumber = updatedTenant.PhoneNumber ?? tenant.PhoneNumber;
            var address = updatedTenant.Address ?? tenant.Address;

            return new Tenant()
            {
                Name = name,
                Email = email,
                PasswordHash = tenant.PasswordHash,
                PasswordSalt = tenant.PasswordSalt,
                appointmentSettings = AppointmentSettings.CreateDefaultInstace(),
                PhoneNumber = phoneNumber,
                Address = address,
            };
        }

        // Create details collection in DB - adding indexes.
        private IMongoCollection<Tenant> CreateDetailsCollection(string dbName)
        {
            var database = _mongoClient.GetDatabase(dbName);
            var tenantsCollection = database.GetCollection<Tenant>(General.Constants.detailsCollectionName);
            var indexes = new[]
            {
               new CreateIndexModel<Tenant>(
                    Builders<Tenant>.IndexKeys.Ascending(t => t.Name),
                    new CreateIndexOptions { Unique = true }
                ),
                 new CreateIndexModel<Tenant>(
                    Builders<Tenant>.IndexKeys.Ascending(t => t.Email),
                    new CreateIndexOptions { Unique = true }
                ),
            };
            tenantsCollection.Indexes.CreateMany(indexes);
            return tenantsCollection;
        }

        // Create users collection in DB - adding indexes.
        private IMongoCollection<User> CreateUsersCollection(string dbName)
        {
            var database = _mongoClient.GetDatabase(dbName);
            var usersCollection = database.GetCollection<User>(General.Constants.usersCollectionName);
            var userIndexes = new[]
            {
                new CreateIndexModel<User>(
                    Builders<User>.IndexKeys.Ascending(u => u.Name),
                    new CreateIndexOptions { Unique = true }
                ),
                 new CreateIndexModel<User>(
                    Builders<User>.IndexKeys.Ascending(u => u.Email),
                    new CreateIndexOptions { Unique = true }
                ),
            };
            usersCollection.Indexes.CreateMany(userIndexes);
            return usersCollection;
        }

        // Create Products collection in DB - adding indexes.
        private IMongoCollection<Product> CreateProductsCollection(string dbName)
        {
            var database = _mongoClient.GetDatabase(dbName);
            var productsCollection = database.GetCollection<Product>(General.Constants.productsCollectionName);
            var productsIndexes = new[]
            {
                new CreateIndexModel<Product>(
                    Builders<Product>.IndexKeys.Ascending(u => u.Name),
                    new CreateIndexOptions { Unique = true }
                ),
            };
            productsCollection.Indexes.CreateMany(productsIndexes);
            return productsCollection;
        }

        // Create Orders collection in DB.
        private IMongoCollection<Order> CreateOrdersCollection(string dbName)
        {
            var database = _mongoClient.GetDatabase(dbName);
            return database.GetCollection<Order>(General.Constants.ordersCollectionName);
        }

        // Create Appointments collection in DB.
        private IMongoCollection<BsonDocument> CreateAppointmentsCollection(string dbName)
        {
            var database = _mongoClient.GetDatabase(dbName);
            return database.GetCollection<BsonDocument>(General.Constants.appointmentsCollectionName);
        }

    }
}
