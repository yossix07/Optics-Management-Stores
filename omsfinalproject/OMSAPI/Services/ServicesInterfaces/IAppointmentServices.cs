using Microsoft.AspNetCore.Mvc;
using OMSAPI.Dto.AppointmentsDto;
using OMSAPI.Models.Entities;

namespace OMSAPI.Services.ServicesInterfaces
{
    public interface IAppointmentServices
    {
        Task<ActionResult<Dictionary<DateOnly, List<AppointmentSlotDto>>>?> GetAppointmentsByDateAndStatus(string tenantId, DateOnly startDate, DateOnly endDate, string status, List<User> users);
        Task<ActionResult<Dictionary<DateOnly, List<AppointmentSlotDto>>>?> GetAllUserAppointments(string tenantId, User user);
        Task<Dictionary<DateOnly, AppointmentSlotDto>?> GetAppointmentById(string tenantId, string appointmentId);
        Task<bool> CreateAppointment(string tenantId, CreateAppointmentDto appointmentSlot);
        Task<bool> DeleteAppointment(string tenantId, string appointmentId);
    }
}
