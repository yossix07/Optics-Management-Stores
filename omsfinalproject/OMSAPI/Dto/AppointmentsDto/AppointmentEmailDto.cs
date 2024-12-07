using OMSAPI.Models.Entities;

namespace OMSAPI.Dto.AppointmentsDto
{
    public class AppointmentEmailDto
    {

        public DateOnly Date { get; set; } 

        public AppointmentSlotDto Slot { get; set; }

        public Tenant Tenant { get; set; }

        public User User { get; set; }

        public AppointmentEmailDto(DateOnly date, AppointmentSlotDto slot, Tenant tenant, User user)
        {
            Date = date;
            Slot = slot;
            Tenant = tenant;
            User = user;
        }
    }
}
