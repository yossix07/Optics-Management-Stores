using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OMSAPI.Services.ServicesInterfaces;
using OMSAPI.General;
using OMSAPI.Models.Appointments;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using OMSAPI.Dto.AppointmentsDto;
using OMSAPI.Models.Entities;

namespace OMSAPI.Services.AppointmentsServices
{
    public class AppointmentServices : IAppointmentServices
    {

        private readonly ILogger<AppointmentServices> _logger;
        private readonly IDatabaseServices _databaseServices;
        private readonly AppointmentSettingsServices _appointmentSettingsServices;
        private readonly IUserServices _userServices;
        private readonly IEmailServices _emailServices;
        private readonly ITenantServices _tenantServices;


        public AppointmentServices(ILogger<AppointmentServices> logger, IDatabaseServices databaseServices, AppointmentSettingsServices appointmentSettingsServices, IUserServices userServices, IEmailServices emailServices, ITenantServices tenantServices)
        {
            _logger = logger;
            _databaseServices = databaseServices;
            _appointmentSettingsServices = appointmentSettingsServices;
            _userServices = userServices;
            _emailServices = emailServices;
            _tenantServices = tenantServices;
        }


        /// <summary>
        /// Return all appointments for spesific tenant. 
        /// </summary>

        public async Task<ActionResult<Dictionary<DateOnly, List<AppointmentSlotDto>>>?> GetAppointmentsByDateAndStatus(string tenantId, DateOnly startDate, DateOnly endDate, string status, List<User> users)
        {

            // Find the collection in the tenant DB.
            var appointmentCollection = _databaseServices.FindCollectionByDB<BsonDocument>(tenantId, Constants.appointmentsCollectionName);
            if (appointmentCollection != null)
            {
                // Creating filter for finding only documents in the date range.
                var filter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Gte("date", startDate.ToString()),
                    Builders<BsonDocument>.Filter.Lte("date", endDate.ToString())
                );
                var documents = await appointmentCollection.Find(filter).ToListAsync();


                // create Dictionary and inserts emptys lists, one for each date.
                var map = new Dictionary<DateOnly, List<AppointmentSlotDto>>();
                var currentDate = startDate;
                while (currentDate <= endDate)
                {
                    map.Add(currentDate, new List<AppointmentSlotDto>());
                    currentDate = currentDate.AddDays(1);
                }

                // Create slots and update the dictionary.
                List<AppointmentSlotDto> currentDateSlots = new List<AppointmentSlotDto>();
                foreach (var doc in documents)
                {
                    var bsonSlots = doc["slots"].AsBsonArray;
                    var dateOnly = Utils.ExtractDateFromBsonDocument(doc);

                    foreach (var bsonSlot in bsonSlots)
                    {
                        var slot = BsonSerializer.Deserialize<AppointmentSlot>(bsonSlot.AsBsonDocument);

                        // Find the user in the users list.
                        if (slot.UserId != null)
                        {
                            var user = FindUserInUsersList(users, slot.UserId);
                            if (user == null)
                            {
                                _logger.LogError($"The user {slot.UserId} was not found in the users list");
                                return null;
                            }
                            if (slot.Status.ToString() == status)
                            {
                                currentDateSlots.Add(new AppointmentSlotDto(slot, user));
                            }
                        }
                        else if (slot.Status.ToString() == status)
                        {
                            currentDateSlots.Add(new AppointmentSlotDto(slot));
                        }
                    }
                    var sortedList = currentDateSlots.OrderBy(slots => slots.StartTime).ToList();
                    map[dateOnly] = new List<AppointmentSlotDto>(sortedList);
                    currentDateSlots.Clear();
                }
                return map;
            }
            _logger.LogError("Faild to find collection in DB");
            return null;
        }


        /// <summary>
        /// Gets all appointments for a given user.
        /// </summary>
        public async Task<ActionResult<Dictionary<DateOnly, List<AppointmentSlotDto>>>?> GetAllUserAppointments(string tenantId, User user)
        {
            // Find the collection in the tenant DB.
            var appointmentCollection = _databaseServices.FindCollectionByDB<BsonDocument>(tenantId, Constants.appointmentsCollectionName);
            if (appointmentCollection != null)
            {

                var filter = Builders<BsonDocument>.Filter.Eq("slots.UserId", user.Id);
                var projection = Builders<BsonDocument>.Projection
                                    .Include("date")
                                    .Include("slots");

                // Return the appointment with Id = appointmentId
                var slotDocument = await appointmentCollection.Find(filter).Project(projection).ToListAsync();
                if (slotDocument != null)
                {
                    var dict = new Dictionary<DateOnly, List<AppointmentSlotDto>>();
                    try
                    {
                        for (int i = 0; i < slotDocument.Count(); i++)
                        {
                            var dateOnly = Utils.ExtractDateFromBsonDocument(slotDocument[i]);
                            List<AppointmentSlotDto> slots = new List<AppointmentSlotDto>();
                            var currentDoc = slotDocument[i]["slots"].AsBsonArray;
                            for (int j = 0; j < currentDoc.Count(); j++)
                            {
                                var temp = currentDoc[j];
                                if (currentDoc[j]["UserId"] != null && currentDoc[j]["UserId"] != BsonNull.Value)
                                {
                                    var slot = AppointmentSlot.CreateSlotFromBsonDocument(slotDocument[i], j);
                                    slots.Add(new AppointmentSlotDto(slot, user));
                                }
                            }
                            dict[dateOnly] = slots;
                        }

                        return dict;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Failed to create slots from Bson document.  Exception {ex}");
                        return null;
                    }
                }
            }
            _logger.LogError("Faild to find collection in DB");
            return null;
        }


        /// <summary>
        /// Gets all appointments with Id = appointmentId
        /// </summary>
        public async Task<Dictionary<DateOnly, AppointmentSlotDto>?> GetAppointmentById(string tenantId, string appointmentId)
        {
            // Find the collection in the tenant DB.
            var appointmentCollection = _databaseServices.FindCollectionByDB<BsonDocument>(tenantId, Constants.appointmentsCollectionName);
            if (appointmentCollection != null)
            {

                var filter = Builders<BsonDocument>.Filter.Eq("slots._id", new ObjectId(appointmentId));
                var projection = Builders<BsonDocument>.Projection
                                    .Include("slots")
                                    .Include("date")
                                    .ElemMatch("slots", Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(appointmentId)));

                // Return the appointment with Id = appointmentId
                var slotDocument = await appointmentCollection.Find(filter).Project(projection).FirstOrDefaultAsync();
                if (slotDocument != null)
                {
                    try
                    {
                        var dict = new Dictionary<DateOnly, AppointmentSlotDto>();
                        var dateOnly = Utils.ExtractDateFromBsonDocument(slotDocument);

                        // initialize slot with default values.
                        var slot = new AppointmentSlot();
                        foreach (var bsonSlot in slotDocument["slots"].AsBsonArray)
                        {
                            var temp = BsonSerializer.Deserialize<AppointmentSlot>(bsonSlot.AsBsonDocument);
                            if (temp.Id == appointmentId)
                            {
                                slot = temp;
                                break;
                            }
                        }

                        if (slot.Status != AppointmentStatus.Available.ToString() && slot.UserId != null)
                        {
                            var user = await _userServices.GetById(tenantId, slot.UserId);
                            if (user == null)
                            {
                                _logger.LogError($"The user {slot.UserId} was not found in the users list");
                                return null;
                            }
                            dict.Add(dateOnly, new AppointmentSlotDto(slot, user));
                            return dict;
                        }
                        dict.Add(dateOnly, new AppointmentSlotDto(slot));
                        return dict;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Failed to create slots from Bson document. Exception {ex}");
                        return null;
                    }
                }
            }
            _logger.LogError("Faild to find collection in DB");
            return null;
        }


        /// <summary>
        /// Creates a new appointment for the given tenant.
        /// </summary>
        public async Task<bool> CreateAppointment(string tenantId, CreateAppointmentDto appointmentSlot)
        {
            // Find the collection in the tenant DB.
            var appointmentCollection = _databaseServices.FindCollectionByDB<BsonDocument>(tenantId, Constants.appointmentsCollectionName);
            if (appointmentCollection != null)
            {
                AppointmentType? type = await _appointmentSettingsServices.GetAppointmentTypesByName(tenantId, appointmentSlot.TypeName);
                if (type == null)
                {
                    return false;
                }
                var filter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.ElemMatch("slots", Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(appointmentSlot.AppointmentId)),
                    Builders<BsonDocument>.Filter.Eq("Status", AppointmentStatus.Available.ToString()))));

                var update = Builders<BsonDocument>.Update
                    .Set("slots.$.Status", AppointmentStatus.Booked.ToString())
                    .Set("slots.$.UserId", appointmentSlot.UserId)
                    .Set("slots.$.Type", type)
                    .Set("slots.$.Description", appointmentSlot.Description);

                // Insert new appointment to DB.
                var appointments = await appointmentCollection.Find(filter).ToListAsync();
                var updated = await appointmentCollection.UpdateOneAsync(filter, update);
                if (updated.ModifiedCount <= 0)
                {
                    _logger.LogError($"Did not find available appointment with Id = {appointmentSlot.AppointmentId}");
                    return false;
                }
                _logger.LogInformation($"Appointment was created for {tenantId}");
                var notify = await NotifyAppointmentCreation(tenantId, appointmentSlot);
                if (!notify)
                {
                    // if notify failed, still return ok.
                    _logger.LogError($"Failed to notify tenant or user");
                }
                return true;
            }
            _logger.LogError("Faild to find collection in DB");
            return false;
        }

        /// <summary>
        /// Delete appointment with id = appointmentId from DB.
        /// <summary>
        public async Task<bool> DeleteAppointment(string tenantId, string appointmentId)
        {
            // save temporary local copy for notifing user and tenant
            var slot = await GetAppointmentById(tenantId, appointmentId);

            // Find the collection in the tenant DB.
            var appointmentCollection = _databaseServices.FindCollectionByDB<BsonDocument>(tenantId, Constants.appointmentsCollectionName);
            if (appointmentCollection != null)
            {
                var filter = Builders<BsonDocument>.Filter.And(
                     Builders<BsonDocument>.Filter.ElemMatch("slots", Builders<BsonDocument>.Filter.And(
                     Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(appointmentId)),
                     Builders<BsonDocument>.Filter.Eq("Status", AppointmentStatus.Booked.ToString()))));

                var update = Builders<BsonDocument>.Update
                    .Set("slots.$.Status", AppointmentStatus.Available.ToString())
                    .Set("slots.$.UserId", BsonNull.Value)
                    .Set("slots.$.Type", BsonNull.Value)
                    .Set("slots.$.Description", BsonNull.Value);


                // Insert new appointment to DB.
                //var appointments = await appointmentCollection.Find(filter).ToListAsync();
                var updated = await appointmentCollection.UpdateOneAsync(filter, update);
                if (updated.ModifiedCount <= 0)
                {
                    _logger.LogError($"Did not find booked appointment with Id = {appointmentId}");
                    return false;
                }
                _logger.LogInformation($"Appointment was Deleted for {tenantId}");
                var notify = await NotifyAppointmentCancelation(tenantId, slot?.Values.First().UserId, slot?.Values.First().Id);
                if (!notify)
                {
                    // if notify failed, still return ok.
                    _logger.LogError($"Failed to notify tenant or user");
                }
                return true;
            }
            _logger.LogError("Faild to find collection in DB");
            return false;
        }

        private User? FindUserInUsersList(List<User> list, string userId)
        {
            if (userId != null)
            {
                foreach (var user in list)
                {
                    if (user.Id == userId)
                    {
                        return user;
                    }
                }
            }
            return null;
        }

        private async Task<bool> NotifyAppointmentCreation(string tenantId, CreateAppointmentDto appointmentSlot)
        {
            // extract data
            var appointment = await GetAppointmentById(tenantId, appointmentSlot.AppointmentId);
            var tenant = await _tenantServices.Get(tenantId);
            var user = await _userServices.GetById(tenantId, appointmentSlot.UserId);

            // validate results
            if (user == null || tenant == null || appointment == null)
            {
                return false;
            }

            // Send emails to tenant and user.
            var send_user  = await _emailServices.NotifyUserAboutAppointmentCreation(user.Email, appointment, tenant, user);
            var send_tenant= await _emailServices.NotifyTenantAboutAppointmentCreation(tenant.Email, appointment, tenant, user);

            // check results.
            if (send_tenant && send_user)
            {
                return true;
            }

            return false;
        }

        private async Task<bool> NotifyAppointmentCancelation(string tenantId, string? userId, string? appointmentId)
        {
            // validate input
            if (userId == null || appointmentId == null)
            {
                return false;
            }

            // extract data 
            var appointment = await GetAppointmentById(tenantId, appointmentId);
            var tenant = await _tenantServices.Get(tenantId);
            var user = await _userServices.GetById(tenantId, userId);

            // validate results
            if (user == null || tenant == null || appointment == null)
            {
                return false;
            }

            // Send emails to tenant and user.
            var send_user = await _emailServices.NotifyUserAboutAppointmentCancelation(user.Email, appointment, tenant, user);
            var send_tenant = await _emailServices.NotifyTenantAboutAppointmentCancelation(tenant.Email, appointment, tenant, user);

            // check result.
            if (send_tenant && send_user)
            {
                return true;
            }

            return false;
        }


    }
}
