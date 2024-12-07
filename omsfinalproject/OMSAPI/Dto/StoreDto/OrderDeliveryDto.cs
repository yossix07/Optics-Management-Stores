using OMSAPI.Models.Store;
using System.ComponentModel.DataAnnotations;

namespace OMSAPI.Dto.StoreDto
{
    public class OrderDeliveryDto
    {

        [Required]
        public string OrderId { get; set; }

        [Required]
        public string Status { get; set; }
    }
}
