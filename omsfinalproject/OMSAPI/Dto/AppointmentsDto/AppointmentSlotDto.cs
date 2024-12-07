using OMSAPI.Models.Appointments;
using OMSAPI.Models.Entities;

namespace OMSAPI.Dto.AppointmentsDto
{
    public class AppointmentSlotDto
    {
        public string Id { get; set; } = String.Empty;
        public string? UserId { get; set; }
        public string? UserName { get; set; } = null;
        public string? UserEmail { get; set; } = null;
        public string? UserPhoneNumber { get; set; } = null;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public string Status { get; set; }
        public AppointmentType? Type { get; set; }
        public string? Description { get; set; } = string.Empty;

        public AppointmentSlotDto(AppointmentSlot slot)
        {
            Id = slot.Id;
            UserId = slot.UserId;
            StartTime = slot.StartTime;
            EndTime = slot.EndTime;
            DayOfWeek = slot.DayOfWeek;
            Status = slot.Status;
            Type = slot.Type;
            Description = slot.Description;
        }
        public AppointmentSlotDto(AppointmentSlot slot, string userName, string userEmail, string userPhoneNumber)
        {
            Id = slot.Id;
            UserId = slot.UserId;
            UserName = userName;
            UserEmail = userEmail;
            UserPhoneNumber = userPhoneNumber;
            StartTime = slot.StartTime;
            EndTime = slot.EndTime;
            DayOfWeek = slot.DayOfWeek;
            Status = slot.Status;
            Type = slot.Type;
            Description = slot.Description;
        }

        public AppointmentSlotDto(AppointmentSlot slot, User user)
        {
            Id = slot.Id;
            UserId = slot.UserId;
            UserName = user.Name;
            UserEmail = user.Email;
            UserPhoneNumber = user.PhoneNumber;
            StartTime = slot.StartTime;
            EndTime = slot.EndTime;
            DayOfWeek = slot.DayOfWeek;
            Status = slot.Status;
            Type = slot.Type;
            Description = slot.Description;
        }


    }
}
