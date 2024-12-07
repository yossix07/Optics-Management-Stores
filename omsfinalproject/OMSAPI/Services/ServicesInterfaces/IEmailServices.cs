using OMSAPI.Dto.AppointmentsDto;
using OMSAPI.Dto.StoreDto;
using OMSAPI.Models.Entities;

namespace OMSAPI.Services.ServicesInterfaces
{
    public interface IEmailServices
    {
        bool SendEmail(string emailAddress, string subject, string body);
        Task<bool> SendUserOrderEmail(string emailAddress, OrderEmailReponseDto respose);
        Task<bool> SendTenantOrderEmail(string emailAddress, OrderEmailReponseDto respose);
        bool SendResetPasswordEmail(string emailAddress, string verificationCode);
        Task<bool> NotifyUserAboutAppointmentCreation(string emailAddress, Dictionary<DateOnly, AppointmentSlotDto> dto, Tenant tenant, User user);
        Task<bool> NotifyTenantAboutAppointmentCreation(string emailAddress, Dictionary<DateOnly, AppointmentSlotDto> dto, Tenant tenant, User user);
        Task<bool> NotifyUserAboutAppointmentCancelation(string emailAddress, Dictionary<DateOnly, AppointmentSlotDto> dto, Tenant tenant, User user);
        Task<bool> NotifyTenantAboutAppointmentCancelation(string emailAddress, Dictionary<DateOnly, AppointmentSlotDto> dto, Tenant tenant, User user);

    }
}
