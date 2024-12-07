using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using OMSAPI.General;

namespace OMSAPI.Models.Appointments
{

    /// <summary>
    /// This class describes the blocks of times in the week that the tenant/store is open to getting appointments.
    /// For example, {09:00,13:00, Sunday},{13:00,16:00, Monday}, etc... 
    /// </summary>
    public class AppointmentsAvailableBlock
    {        
        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }
        public DayOfWeek DayOfWeek { get; set; }

        public AppointmentsAvailableBlock()
        {
            StartTime = default(TimeOnly);
            EndTime = default(TimeOnly);
            DayOfWeek = default(DayOfWeek);
        }
        public AppointmentsAvailableBlock(TimeOnly startTime, TimeOnly endTime, DayOfWeek dayOfWeek)
        {
            StartTime = startTime;
            EndTime = endTime;
            DayOfWeek = dayOfWeek;
        }

        public override string ToString()
        {
            return $"{DayOfWeek}: {StartTime.ToString()} - {EndTime.ToString()}";
        }

    }
}
