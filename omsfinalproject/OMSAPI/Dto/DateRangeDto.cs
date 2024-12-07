using OMSAPI.Dto.AppointmentsDto;
using OMSAPI.Models.Appointments;
using System.ComponentModel.DataAnnotations;

namespace OMSAPI.Dto
{
    public class DateRangeDto
    {
        [Required]
        public DateDto Start { get; set; }

        [Required]
        public DateDto End { get; set; }

        public DateRangeDto(DateDto start, DateDto end)
        {
            Start = start;
            End = end;
        }

        public DateRangeDto() { }

        public DateRangeWithStatusDto CreateRangeWithAvailableStatus()
        {
            return new DateRangeWithStatusDto(Start, AppointmentStatus.Available, End);
        }

        // check 
        public bool ValidateDateRangoDto()
        {
            if (Start.convertToDateOnly() < End.convertToDateOnly())
            {
                return true;
            }

            return false;
        }
    }
}
