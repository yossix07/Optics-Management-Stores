using System.ComponentModel.DataAnnotations;

namespace OMSAPI.Dto.AppointmentsDto
{
    public class HolidayDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        [Range(1, 12, ErrorMessage = "Month must be between 1 and 12")]
        public int Month { get; set; }

        [Required]
        [Range(1, 31, ErrorMessage = "Day must be between 1 and 31")]
        public int Day { get; set; }


        public HolidayDto(int year, int month, int day)
        {
            Year = year;
            Month = month;
            Day = day;
        }

        public DateOnly convertToDateOnly()
        {
            return new DateOnly(Year, Month, Day);
        }
    }
}
