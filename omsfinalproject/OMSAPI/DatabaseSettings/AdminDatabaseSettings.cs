using OMSAPI.AccessDatabase;

namespace OMSAPI.DatabaseSettings
{
    public class AdminDatabaseSettings : IDatabaseSettings
    {
        public string CollectionName { get; set; } = String.Empty;
        public string ConnectionString { get; set; } = String.Empty;
        public string DatabaseName { get; set; } = String.Empty;
    }
}
