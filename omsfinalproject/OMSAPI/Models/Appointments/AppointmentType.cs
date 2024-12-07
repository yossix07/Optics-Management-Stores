using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace OMSAPI.Models.Appointments
{
    public class AppointmentType
    {
        [Required(ErrorMessage = "Type name is required.")]
        public string TypeName { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        public decimal Price { get; set; }
    }

}
