using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using OMSAPI.Models.Entities;

namespace OMSAPI.Models.Appointments
{
    public class AppointmentSlot
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = String.Empty;
        public string? UserId { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public string Status { get; set; }
        public AppointmentType? Type { get; set; }
        public string? Description { get; set; } = string.Empty;

        public AppointmentSlot()
        {
            Id = string.Empty;
            UserId = null;
            StartTime = new TimeOnly();
            EndTime = new TimeOnly();
            DayOfWeek = DayOfWeek.Sunday;
            Status = string.Empty;
            Type = null;
            Description = string.Empty;
        }

        public AppointmentSlot(TimeOnly startTime, TimeOnly endTime, DayOfWeek dayOfWeek, AppointmentStatus status)
        {
            Id = ObjectId.GenerateNewId().ToString();
            UserId = null;
            StartTime = startTime;
            EndTime = endTime;
            DayOfWeek = dayOfWeek;
            Status = status.ToString();
            Type = null;
            Description = null;
        }

        public AppointmentSlot(string id, string? userId, TimeOnly startTime, TimeOnly endTime, DayOfWeek dayOfWeek, string status, AppointmentType? type, string? description)
        {
            Id = id;
            UserId = userId;
            StartTime = startTime;
            EndTime = endTime;
            DayOfWeek = dayOfWeek;
            Status = status;
            Type = type;
            Description = description;
        }

        private static TimeOnly ConvertInt64ToTimeOnly(BsonDocument slotDocument, string attribueName, int index = 0)
        {
            var ticks = slotDocument["slots"][index][attribueName].AsInt64;
            var res = new DateTime(ticks);
            return TimeOnly.FromDateTime(res);
        }

        public static AppointmentSlot CreateSlotFromBsonDocument(BsonDocument slotDocument, int index)
        {
            var startTime = ConvertInt64ToTimeOnly(slotDocument, "StartTime", index);
            var endTime = ConvertInt64ToTimeOnly(slotDocument, "EndTime", index);
            return new AppointmentSlot(
                id: slotDocument["slots"][index]["_id"]?.ToString() ?? throw new ArgumentNullException(slotDocument["_id"].ToString()),
                startTime: startTime,
                endTime: endTime,
                dayOfWeek: (DayOfWeek)slotDocument["slots"][index]["DayOfWeek"].ToInt32(),
                status: slotDocument["slots"][index]["Status"]?.ToString() ?? throw new ArgumentNullException(slotDocument["Status"].ToString()),
                type: slotDocument["slots"][index]["Type"] != null ? new AppointmentType
                {
                    TypeName = slotDocument["slots"][index]["Type"]["TypeName"].AsString,
                    Price = slotDocument["slots"][index]["Type"]["Price"].ToDecimal()
                } : null,
                description: slotDocument["slots"][index]["Description"]?.ToString() ?? null,
                userId: slotDocument["slots"][index]["UserId"]?.ToString() ?? throw new ArgumentNullException(slotDocument["Status"].ToString())
            );
        }
    }
}
