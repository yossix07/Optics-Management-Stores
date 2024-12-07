using MongoDB.Driver;
using OMSAPI.AccessDatabase;
using OMSAPI.Dto.EntitiesDto;
using OMSAPI.Models.Entities;
using OMSAPI.Services.ServicesInterfaces;

namespace OMSAPI.Services.EntitiesServices
{
    public class AdminServices: IAdminServices
    {
        private readonly IMongoCollection<Admin>? _adminCollection;
        private readonly ILogger<IAdminServices> _logger;
        private readonly IDatabaseServices _databaseServices;
        public AdminServices(IDatabaseSettings settings, IMongoClient mongoClient, ILogger<IAdminServices> logger, IDatabaseServices databaseServices)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _logger = logger;
            _databaseServices = databaseServices;
            _adminCollection = _databaseServices.FindCollectionByDB<Admin>(General.Constants.mainDbName, General.Constants.mainDbAdminCollectionName);            
        }

        public Admin CreateAdminFromDto(AdminDto admin, byte[] passwordHash, byte[] passwordSalt)
        {

            return new Admin()
            {
                Name = admin.Name,
                Email = admin.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
            };
        }

        public async Task<Admin?> GetAdminByName(string name)
        {
            return await _adminCollection.Find(admin => admin.Name == name).FirstOrDefaultAsync();
        }

        public async Task<string?> Create(Admin admin)
        {
            try
            {
                // Create new admin in collection.
                await _adminCollection.InsertOneAsync(admin);
            }
            catch (MongoWriteException ex)
            {
                if (ex.Message.Contains("name_1"))
                {
                    _logger.LogError($"{admin.Name} already exists in the system");
                    return admin.Name;
                }
                else if (ex.Message.Contains("email_1"))
                {
                    _logger.LogError($"{admin.Email} already exists in the system");
                    return admin.Email;
                }
            }
            catch
            {
                _logger.LogError($"Faild to create tenant {admin.Name}");
                return "Failed";
            }

            return null;
        }

        public async Task<List<Admin>> GetAll()
        {
            return await _adminCollection.Find(admin => true).ToListAsync();
        }

        public async Task DeleteAdmin(Admin admin1)
        {
            await _adminCollection.DeleteOneAsync(admin => admin.Id == admin1.Id);
        }
    }
}
