using OMSAPI.Models.Appointments;
using OMSAPI.Models.Entities;

namespace OMSAPI.Services.ServicesInterfaces
{
    public interface IAppointmentSettingsServices
    {
        public Task<bool?> CreateAppointmentType(string tenantId, AppointmentType appointmentType);
        public Task<AppointmentType?> GetAppointmentTypesByName(string dbName, string typeName);

        public Task<bool> ValidateAvailableBlock(string tenantId, AppointmentsAvailableBlock newBlock);

        public Task<TimeSpan?> GetSlotDuration(string dbName);

        public Task<Dictionary<DateOnly, List<AppointmentSlot>>?> GenerateSlots(string tenantId, DateTime start, DateTime end, AppointmentsAvailableBlock? appointmentBlock = null, TimeSpan? slotDuration = null);
        public Task<bool?> UpdateSlotDuration(string dbName, TimeSpan duration);

        public Task<bool> PutAppointmentType(string dbName, AppointmentType appointmentType);

        public Task<bool> DeleteAppointmentType(string dbName, string typeName);
        public Task<List<Holiday>?> GetAllDaysOff(string dbName);

        public Task<bool> CreateDayOff(string dbName, Holiday holiday);

        public Task<bool> DeleteDayOffByDate(string dbName, DateOnly date);

        public Task<bool> CreateAvailableBlock(string dbName, AppointmentsAvailableBlock block);

        public Task<bool> DeleteAvailableBlock(string dbName, AppointmentsAvailableBlock block);

        public Task<List<AppointmentsAvailableBlock>?> GetAllAvailableBlocks(string dbName);

        public List<AppointmentType>? GetAllTypes(Tenant tenant);


    }
}
