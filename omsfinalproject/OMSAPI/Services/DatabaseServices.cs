using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OMSAPI.DatabaseSettings;
using OMSAPI.Models.Entities;
using OMSAPI.Services.EntitiesServices;
using OMSAPI.Services.ServicesInterfaces;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace OMSAPI.Services
{
    public class DatabaseServices: IDatabaseServices
    {
        private readonly IMongoClient _mongoClient;
        private IConfiguration _configuration;
        private readonly ILogger<DatabaseServices> _logger;

        public DatabaseServices(IOptions<AdminDatabaseSettings> databaseSettings, ILogger<DatabaseServices> logger, IConfiguration configuration)
        {
            _mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
            _logger = logger;
            _configuration = configuration;
        }


        public IMongoCollection<T>? FindCollectionByDB<T>(string dbName, string collectionName)
        {
            if (!_mongoClient.ListDatabaseNames().ToList().Contains(dbName))
            {
                if (dbName == General.Constants.mainDbName)
                {
                    var database = CreateMainDatabase();
                    return database.GetCollection<T>(collectionName);
                }

               _logger.LogInformation($"Cannot find database {dbName}");
                return null;
            }
            
                var db = _mongoClient.GetDatabase(dbName);
                return db.GetCollection<T>(collectionName);
        }

        private IMongoDatabase CreateMainDatabase()
        {
            // Create the main DB and tenants collection
            var database = _mongoClient.GetDatabase(General.Constants.mainDbName);
            var tenantCollection = database.GetCollection<Tenant>(General.Constants.mainDbTenantCollectionName);
            var adminCollection = database.GetCollection<Admin>(General.Constants.mainDbAdminCollectionName);

            // Create and insert collection indexes for Tenants and Admins collection.
            var tenantIndex = CreateTenantIndexes();
            var adminIndex = CreateAdminIndexes();
            tenantCollection.Indexes.CreateMany(tenantIndex);
            adminCollection.Indexes.CreateMany(adminIndex);

            // Create the default admin, with the default password. take data from appsettings.json DefaultAdminDetails
            var defaultAdmin = CreateDefaultAdminForMainDB();
            try
            {
                if (defaultAdmin != null)
                {
                    adminCollection.InsertOne(defaultAdmin);
                    _logger.LogInformation("Default admin created in the main DB");
                }
            } catch (Exception e)
            {
                _logger.LogError($"Cannot create default admin for main DB, {e.Message}");
            }
            
            return database;
        }

        private Admin? CreateDefaultAdminForMainDB()
        {
            var name = _configuration.GetSection("DefaultAdminDetails:Name").Value;
            var email = _configuration.GetSection("DefaultAdminDetails:Email").Value;
            var password = _configuration.GetSection("DefaultAdminDetails:Password").Value;

            if (name == null || email == null || password == null)
            {
                _logger.LogError("Cannot create default admin, missing details in appsettings.json");
                return null;
            }

            // create salt and hash the password, using cryptography algorithm
            var passwordSalt = new byte[128];
            var passwordHash = new byte[128];
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

            return new Admin
            {
                Name = name,
                Email = email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
            };
        }

        private CreateIndexModel<Tenant>[] CreateTenantIndexes()
        {
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

            return indexes;
        }

        private CreateIndexModel<Admin>[] CreateAdminIndexes()
        {
            var indexes = new[]
            {
                new CreateIndexModel<Admin>(
                    Builders<Admin>.IndexKeys.Ascending(t => t.Name),
                    new CreateIndexOptions { Unique = true }
                ),
                    new CreateIndexModel<Admin>(
                    Builders<Admin>.IndexKeys.Ascending(t => t.Email),
                    new CreateIndexOptions { Unique = true }
                ),
            };

            return indexes;
        }



    }
}
