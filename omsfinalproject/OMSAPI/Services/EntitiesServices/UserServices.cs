using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OMSAPI.DatabaseSettings;
using OMSAPI.Services.ServicesInterfaces;
using OMSAPI.Models.Entities;
using OMSAPI.Dto.EntitiesDto;

namespace OMSAPI.Services.EntitiesServices
{
    public class UserServices : IUserServices
    {
        private readonly IMongoClient _mongoClient;
        private readonly string collectionName = General.Constants.usersCollectionName;
        private readonly IDatabaseServices _databaseServices;
        private readonly ILogger<UserServices> _logger;

        public UserServices(IOptions<AdminDatabaseSettings> databaseSettings, IDatabaseServices databaseServices, ILogger<UserServices> logger)
        {
            _mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
            _databaseServices = databaseServices;
            _logger = logger;
        }

        public async Task<User?> GetById(string dbName, string userId)
        {

            var collection = _databaseServices.FindCollectionByDB<User>(dbName, collectionName);

            if (collection == null)
            {
                _logger.LogError($"The collection {collectionName} was not fould in the database {dbName}");
                return null;
            }

            return await collection.Find(user => user.Id == userId).FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserByEmail(string dbName, string email)
        {
            var collection = _databaseServices.FindCollectionByDB<User>(dbName, collectionName);

            if (collection == null)
            {
                _logger.LogError($"The collection {collectionName} was not fould in the database {dbName}");
                return null;
            }

            return await collection.Find(user => user.Email == email).FirstOrDefaultAsync();
        }


        public async Task<List<User>?> GetAll(string dbName)
        {
            var collection = _databaseServices.FindCollectionByDB<User>(dbName, collectionName);

            if (collection == null)
            {
                _logger.LogError($"The collection {collectionName} was not fould in the database {dbName}");
                return null;
            }

            return await collection.Find(user => true).ToListAsync();
        }

        public async Task<string?> Create(string dbName, User user)
        {
            var collection = _databaseServices.FindCollectionByDB<User>(dbName, collectionName);

            if (collection == null)
            {
                _logger.LogError($"The collection {collectionName} was not fould in the database {dbName}", dbName, collectionName);
                return null;
            }

            try
            {
                await collection.InsertOneAsync(user);
            }
            catch (MongoWriteException ex)
            {
                if (ex.Message.Contains("_id_"))
                {
                    _logger.LogError($"{user.Id} already exists in the system");
                    return user.Id.ToString();
                }
                else if (ex.Message.Contains("email_1"))
                {
                    _logger.LogError($"{user.Email} already exists in the system");
                    return user.Email.ToString();
                }
            }
            catch
            {
                _logger.LogError($"Faild to create tenant {user.Id}");
                return "Failed";
            }
            _logger.LogInformation($"User {user.Id} created", user.Id);
            return null;
        }


        public async Task<User?> Update(string dbName, string id, UpdateUserDto userDto)
        {
            User? oldUser = await GetById(dbName, id);

            if (oldUser == null)
            {
                return null;
            }
            User? newUser = CreateUserFromUpdateDto(oldUser, userDto);

            if (newUser == null)
            {
                _logger.LogError($"User {collectionName} does not exists in {dbName}", id, dbName);
                return null;
            }

            var collection = _databaseServices.FindCollectionByDB<User>(dbName, collectionName);

            if (collection == null)
            {
                _logger.LogError($"The collection {collectionName} was not fould in the database {dbName}", dbName, collectionName);
                return null;
            }


            await collection.ReplaceOneAsync(u => u.Id == id, newUser);
            _logger.LogInformation($"User {newUser.Id} updated", newUser.Id);
            return newUser;
        }

        public async Task<string?> Delete(string dbName, string id)
        {
            var collection = _databaseServices.FindCollectionByDB<User>(dbName, collectionName);

            if (collection == null)
            {
                _logger.LogError($"The collection {collectionName} was not fould in the database {dbName}", dbName, collectionName);
                return null;
            }

            await collection.DeleteOneAsync(user => user.Id == id);
            _logger.LogInformation($"User {id} deleted", id);
            return id;
        }


        public User CreateUserFromDto(UserDto userDto, byte[] passwordHash, byte[] passwordSalt)
        {
            return new User()
            {
                Id = userDto.Id,
                Name = userDto.Name,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber,
                DateOfBirth = userDto.DateOfBirth,
            };
        }

        public User CreateUserFromUpdateDto(User user, UpdateUserDto updatedUser)
        {
            var name = updatedUser.Name ?? user.Name;
            var email = updatedUser.Email ?? user.Email;
            var phoneNumber = updatedUser.PhoneNumber ?? user.PhoneNumber;


            return new User()
            {
                Id = user.Id,
                Name = name,
                PasswordHash = user.PasswordHash,
                PasswordSalt = user.PasswordSalt,
                Email = email,
                PhoneNumber = phoneNumber,
                DateOfBirth = user.DateOfBirth,
            };
        }
    }
}
