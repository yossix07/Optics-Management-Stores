using MongoDB.Bson.Serialization.Attributes;
using OMSAPI.Models.Appointments;
using System.ComponentModel.DataAnnotations;

namespace OMSAPI.Dto.AppointmentsDto
{
    public class AppointmentsAvailableBlockDto
    {
        [Required]
        public int StartHour { get; set; }

        [Required]
        public int StartMinute { get; set; }

        [Required]
        public int EndHour { get; set; }

        [Required]
        public int EndMinute { get; set; }

        [Required]
        public DayOfWeek weekDay { get; set; }

        public AppointmentsAvailableBlockDto(int startHour, int startMinute, int endHour, int endMinute, DayOfWeek weekDay)
        {
            StartHour = startHour;
            StartMinute = startMinute;
            EndHour = endHour;
            EndMinute = endMinute;
            this.weekDay = weekDay;
        }

        public TimeOnly CreateTimeOnly(int hour, int minute)
        {
            return new TimeOnly(hour, minute);
        }

        public AppointmentsAvailableBlock? CreateAvailableBlock()
        {
            TimeOnly StartTime = CreateTimeOnly(StartHour, StartMinute);
            TimeOnly EndTime = CreateTimeOnly(EndHour, EndMinute);
            if (EndTime < StartTime)
            {
                return null;
            }
            return new AppointmentsAvailableBlock(StartTime, EndTime, weekDay);

        }

        public override string? ToString()
        {
            return $"[{StartHour} : {StartMinute}, - {EndHour}: {EndMinute}] - {weekDay}";
        }
    }
}
