using OMSAPI.Models.Appointments;
using System.ComponentModel.DataAnnotations;

namespace OMSAPI.Dto.AppointmentsDto
{
    public class DateRangeWithStatusDto
    {
        [Required]
        public DateDto Start { get; set; }

        [Required]
        public AppointmentStatus Status { get; set; }

        [Required]
        public DateDto End { get; set; }

        public DateRangeWithStatusDto()
        {
            // Do not remove this ctor - using for mongoDB seralization
        }

        public DateRangeWithStatusDto(DateDto start, AppointmentStatus status, DateDto end)
        {
            Start = start;
            End = end;
            Status = status;
        }
    }
}
