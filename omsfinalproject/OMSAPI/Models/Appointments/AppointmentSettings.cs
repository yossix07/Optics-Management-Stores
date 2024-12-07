using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json.Serialization;
using static OMSAPI.Models.Appointments.HolidayFetcher;

namespace OMSAPI.Models.Appointments
{
    public class AppointmentSettings
    {
        [Required]
        public TimeSpan SlotDuration { get; set; }

        public List<AppointmentsAvailableBlock> AppointmentsBlocks { get; set; } = new List<AppointmentsAvailableBlock>();

        public List<AppointmentType> AppointmentTypes { get; set; } = new List<AppointmentType>();

        public List<Holiday> DaysOff { get; set; } = new List<Holiday>();


        // Default constructor for AppointmentSettings
        internal AppointmentSettings(bool use)
        {
            //Define default slotDuratoion to 15 minuts.
            SlotDuration = new TimeSpan(0, 15, 0);

            // Define new empty lists.
            AppointmentsBlocks = new List<AppointmentsAvailableBlock>();
            AppointmentTypes = new List<AppointmentType>();

            //initialize DaysOff
            LoadHolidays(DateTime.Now.Year, "IL").Wait();
           
        }


        // Create default AppointmentSettings object.
        public static AppointmentSettings CreateDefaultInstace()
        {
            return new AppointmentSettings(true);
        }


        // Constructor, only slotDuration changed from the default.
        public AppointmentSettings(TimeSpan slotDuration)
        {
            SlotDuration = slotDuration;

            // Define new empty lists.
            AppointmentsBlocks = new List<AppointmentsAvailableBlock>();
            AppointmentTypes = new List<AppointmentType>();

            //initialize DaysOff
            LoadHolidays(DateTime.Now.Year, "IL").Wait();
        }


        // Constructor, all filed are already defined.
        public AppointmentSettings(TimeSpan slotDuration, List<AppointmentsAvailableBlock> appointmentsBlocks, List<AppointmentType> appointmentTypes, List<Holiday> daysOff)
        {
            SlotDuration = slotDuration;
            AppointmentsBlocks = appointmentsBlocks;
            AppointmentTypes = appointmentTypes;
            DaysOff = daysOff;
        }

       
        
        // Find all holidays in a spesific country, those will be the days off.
        public async Task LoadHolidays(int year, string countryCode="IL")
        {
            var holidays = await HolidayFetcher.GetHolidays(year, countryCode);
            if (holidays != null)
            {
                DaysOff = holidays;
                return;
            }
            DaysOff = new List<Holiday>();
        }
    }
}
