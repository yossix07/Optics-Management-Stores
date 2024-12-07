using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using OMSAPI.Models.Appointments;
using System.ComponentModel.DataAnnotations;

namespace OMSAPI.Dto.AppointmentsDto
{
    public class CreateAppointmentDto
    {
        [Required]
        [BsonRepresentation(BsonType.ObjectId)]
        public string AppointmentId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string TypeName { get; set; }

        public string Description { get; set; } = string.Empty;


        public CreateAppointmentDto(string appointmentId, string userId, string typeName, string description)
        {
            AppointmentId = appointmentId;
            UserId = userId;
            TypeName = typeName;
            Description = description;
        }

    }
}
