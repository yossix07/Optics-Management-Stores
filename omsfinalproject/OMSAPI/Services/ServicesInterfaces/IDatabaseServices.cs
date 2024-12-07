using MongoDB.Driver;
using OMSAPI.Models;

namespace OMSAPI.Services.ServicesInterfaces
{
    public interface IDatabaseServices
    {
        //IMongoCollection<T> GetCollection<T>(string database, string collection);
        IMongoCollection<T>? FindCollectionByDB<T>(string dbName, string collectionName);

    }
}
