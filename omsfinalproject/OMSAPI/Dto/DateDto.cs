using System.ComponentModel.DataAnnotations;

namespace OMSAPI.Dto
{
    public class DateDto
    {
        [Required]
        [Range(1000, 9999, ErrorMessage = "Year must be represented with 4 digits")]

        public int Year { get; set; }

        [Required]
        [Range(1, 12, ErrorMessage = "Month must be between 1 and 12")]
        public int Month { get; set; }

        [Required]
        [Range(1, 31, ErrorMessage = "Day must be between 1 and 31")]
        public int Day { get; set; }

        public DateDto()
        {
            // Do not remove this ctor - using for mongoDB seralization
        }
        public DateDto(int year, int month, int day)
        {
            Year = year;
            Month = month;
            Day = day;
        }

        public DateOnly convertToDateOnly()
        {
            return new DateOnly(Year, Month, Day);
        }

        public DateTime ConvertToDateTime()
        {
            return new DateTime(Year, Month, Day);
        }
    }
}
