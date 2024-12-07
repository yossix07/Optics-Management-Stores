using OMSAPI.Models.Entities;

namespace OMSAPI.Dto.StoreDto
{
    public class OrderEmailReponseDto
    {

        public OrderResponseDto OrderResponseDto { get; set; }

        public Tenant Tenant { get; set; }

        public OrderEmailReponseDto(OrderResponseDto orderResponseDto, Tenant tenant)
        {
            OrderResponseDto = orderResponseDto;
            Tenant = tenant;
        }
    }
}
