using System.ComponentModel.DataAnnotations;

namespace OMSAPI.Dto
{
    public class SlotDurationDto
    {
        [Required(ErrorMessage = "Hours is required")]
        public int Hours { get; set; }

        [Required(ErrorMessage = "Minutes is required")]
        [Range(0, 59, ErrorMessage = "Minutes must be between 0 and 59")]
        public int Minutes { get; set; }
    }
}
