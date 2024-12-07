using OMSAPI.Services.ServicesInterfaces;
using MongoDB.Driver;
using OMSAPI.Models.Appointments;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using OMSAPI.General;
using OMSAPI.Models.Entities;

namespace OMSAPI.Services.AppointmentsServices
{
    public class AppointmentSettingsServices : IAppointmentSettingsServices
    {
        private readonly ILogger<AppointmentSettingsServices> _logger;
        private readonly IDatabaseServices _databaseServices;

        public AppointmentSettingsServices(ILogger<AppointmentSettingsServices> logger, IDatabaseServices databaseServices)
        {
            _logger = logger;
            _databaseServices = databaseServices;
        }

        /// <summary>
        /// The funciton receives dbName(tenantId) and typeName of the Appointment type and return the entire object.
        /// </summary>
        public async Task<AppointmentType?> GetAppointmentTypesByName(string dbName, string typeName)
        {
            var detailsCollection = _databaseServices.FindCollectionByDB<Tenant>(dbName, Constants.detailsCollectionName);

            var tenant = await detailsCollection.Find(d => d.Id == dbName).FirstOrDefaultAsync();

            if (tenant.appointmentSettings != null)
            {
                var appointmentTypes = tenant.appointmentSettings.AppointmentTypes;

                foreach (var type in appointmentTypes)
                {
                    if (type.TypeName == typeName)
                    {
                        return type;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// The function returns all the AppointmentType for spesific tenant (tenantId)
        /// The function will take the data from the details collection in the tenant database.
        /// </summary>
        public List<AppointmentType>? GetAllTypes(Tenant tenant)
        {
            try
            {
                // Find appointment types in the tenant database
                if (tenant.appointmentSettings != null)
                {
                    var appointmentTypes = tenant.appointmentSettings.AppointmentTypes;
                    List<AppointmentType> allTypes = new List<AppointmentType>();
                    foreach (var type in appointmentTypes)
                    {
                        allTypes.Add(type);
                    }
                    return allTypes;
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Execption! GetAllTypes throws an exception for {tenant.Id}. Failed to get all Appointment types." +
                    $"Exception info: {ex}");
                return null;
            }
        }


        /// <summary>
        /// Thif function updates 2 databases, one is the main Database and the second one is dbName.
        /// </summary>
        /// <typeparam name="T">Type of document - which object to look for</typeparam>
        /// <param name="dbName">The second DB name (tenantId)</param>
        /// <param name="filter">Filter rule for DB</param>
        /// <param name="update">Update rule for DB</param>
        /// <param name="collectionName">The name of the collection in dbName database.</param>
        /// <returns></returns>
        private async Task<bool> UpdateCollection<T>(string dbName, FilterDefinition<T> filter, UpdateDefinition<T> update, string collectionName, string calledFuncName)
        {
            var generalTenantCollection = _databaseServices.FindCollectionByDB<T>(Constants.mainDbName, Constants.mainDbTenantCollectionName);
            var collection = _databaseServices.FindCollectionByDB<T>(dbName, collectionName);

            if (collection == null || generalTenantCollection == null)
            {
                if (generalTenantCollection == null)
                {
                    _logger.LogError($"The tenant {dbName} was not found in the main database");
                    return false;
                }
                _logger.LogError($"The collection {collectionName} was not found in the database {dbName}");
                return false;
            }

            var tenantDbResult = await collection.UpdateOneAsync(filter, update);
            var mainResult = await generalTenantCollection.UpdateOneAsync(filter, update);

            // TODO:: need to add a retry mechanism when only one of them failed.
            // Checking if both databases have been updated.
            if (tenantDbResult.ModifiedCount > 0 && mainResult.ModifiedCount > 0)
            {
                _logger.LogInformation($"The record is updated in both {dbName} and in the main database. Called function [{calledFuncName}]");
                return true;
            }
            else if (tenantDbResult.ModifiedCount > 0)
            {
                _logger.LogError($"The record is updated only in the {dbName} database and not in the main database. Called function [{calledFuncName}]");
                return false;
            }
            else if (mainResult.ModifiedCount > 0)
            {
                _logger.LogError($"The record is updated only in the main database and not in the {dbName} database. Called function [{calledFuncName}]");
                return false;
            }
            else
            {
                _logger.LogError($"The record was NOT updated in both databases. Called function [{calledFuncName}]");
                return false;
            }
        }

        /// <summary>
        /// This function creates a specific appointment type for a specific tenant.
        /// </summary>
        public async Task<bool?> CreateAppointmentType(string tenantId, AppointmentType appointmentType)
        {
            AppointmentType? type = await GetAppointmentTypesByName(tenantId, appointmentType.TypeName);
            if (type != null)
            {
                return null;
            }
            var filter = Builders<Tenant>.Filter.Eq(x => x.Id, tenantId);
            var update = Builders<Tenant>.Update.Push(x => x.appointmentSettings.AppointmentTypes, appointmentType);

            return await UpdateCollection(tenantId, filter, update, Constants.detailsCollectionName, "CreateAppointmentType");
        }

        /// <summary>
        /// This function deletes a specific appointment type for a specific tenant.
        /// </summary>
        public async Task<bool> DeleteAppointmentType(string dbName, string appointmentTypeName)
        {
            var filter = Builders<Tenant>.Filter.Eq(x => x.Id, dbName);
            var update = Builders<Tenant>.Update.PullFilter(x => x.appointmentSettings.AppointmentTypes, y => y.TypeName == appointmentTypeName);

            return await UpdateCollection(dbName, filter, update, Constants.detailsCollectionName, "DeleteAppointmentType");
        }

        /// <summary>
        /// This function updates a specific appointment type for a specific tenant.
        /// </summary>
        public async Task<bool> PutAppointmentType(string dbName, AppointmentType appointmentType)
        {
            var filter = Builders<Tenant>.Filter.And(
             Builders<Tenant>.Filter.Eq(x => x.Id, dbName),
             Builders<Tenant>.Filter.ElemMatch(x => x.appointmentSettings.AppointmentTypes, at => at.TypeName == appointmentType.TypeName)
            );

            var update = Builders<Tenant>.Update
                .Set($"{nameof(Tenant.appointmentSettings)}.{nameof(AppointmentSettings.AppointmentTypes)}.$", appointmentType)
                .Unset($"{nameof(Tenant)}.{nameof(Tenant.appointmentSettings)}.{nameof(AppointmentSettings.AppointmentTypes)}.$");

            return await UpdateCollection(dbName, filter, update, Constants.detailsCollectionName, "PutAppointmentType");
        }

        /// <summary>
        /// This function returns all the days off for a specific tenant.
        /// </summary>
        public async Task<List<Holiday>?> GetAllDaysOff(string dbName)
        {
            try
            {
                var detailsCollection = _databaseServices.FindCollectionByDB<Tenant>(dbName, Constants.detailsCollectionName);
                var tenant = await detailsCollection.Find(d => d.Id == dbName).FirstOrDefaultAsync();
                if (tenant.appointmentSettings != null)
                {
                    return tenant.appointmentSettings.DaysOff.OrderBy(holiday => holiday.Date).ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Execption! GetAllDaysOff throws an exception for {dbName}. Failed to get all days off." +
                    $"Exception info: {ex}");
                return null;
            }
        }

        /// <summary>
        /// This function creates a specific day off for a specific tenant.
        /// </summary>
        public async Task<bool> CreateDayOff(string dbName, Holiday holiday)
        {
            var filter = Builders<Tenant>.Filter.Eq(x => x.Id, dbName);
            var update = Builders<Tenant>.Update.Push(x => x.appointmentSettings.DaysOff, holiday);

            var result = await UpdateCollection(dbName, filter, update, Constants.detailsCollectionName, "CreateDayOff");

            if (!result)
            {
                _logger.LogError($"Failed to create a day off for {dbName}.");
                return false;
            }

            return await DeleteSlotsByDate(dbName, holiday.Date);

        }

        /// <summary>
        /// This function deletes a specific day off for a specific tenant.
        /// </summary>
        public async Task<bool> DeleteDayOffByDate(string dbName, DateOnly date)
        {
            var filter = Builders<Tenant>.Filter.And(
                Builders<Tenant>.Filter.Eq(x => x.Id, dbName),
                Builders<Tenant>.Filter.ElemMatch(x => x.appointmentSettings.DaysOff, y => y.Date == date)
            );

            var update = Builders<Tenant>.Update.PullFilter(x => x.appointmentSettings.DaysOff, y => y.Date == date);

            var result = await UpdateCollection(dbName, filter, update, Constants.detailsCollectionName, "DeleteDayOffByDate");

            if (!result)
            {
                _logger.LogError($"Failed to create a day off for {dbName}.");
                return false;
            }

            // convert Dateonly to DateTime 
            DateTime dateTime = new DateTime(date.Year, date.Month, date.Day);
            var newSlots = GenerateSlots(dbName, dateTime, dateTime);
            if (newSlots != null)
            {
                _logger.LogInformation($"Delete day off finished successfully");
                return true;
            }
            else
            {
                _logger.LogError($"Failed to create new slots after deleting a day off for {dbName}.");
                return false;
            }

        }

        /// <summary>
        /// This function creates a specific Appointment Available Block for a specific tenant.
        /// </summary>
        public async Task<bool> CreateAvailableBlock(string dbName, AppointmentsAvailableBlock block)
        {
            var filter = Builders<Tenant>.Filter.Eq(x => x.Id, dbName);
            var update = Builders<Tenant>.Update.Push(x => x.appointmentSettings.AppointmentsBlocks, block);

            var result = await UpdateCollection(dbName, filter, update, Constants.detailsCollectionName, "CreateAvailableBlock");

            if (result)
            {
                await GenerateSlots(dbName, DateTime.Today, DateTime.Today.AddMonths(Constants.availableBlockMothsDuration), block);
            }

            _logger.LogInformation($"Create available block finished successfully");
            return true;

        }

        /// <summary>
        /// This function deletes specific Appointment Available Block for a specific tenant.
        /// </summary>
        public async Task<bool> DeleteAvailableBlock(string dbName, AppointmentsAvailableBlock block)
        {
            var filter = Builders<Tenant>.Filter.And(
                Builders<Tenant>.Filter.Eq(x => x.Id, dbName),
                Builders<Tenant>.Filter.ElemMatch(x => x.appointmentSettings.AppointmentsBlocks,
                    ab => ab.DayOfWeek == block.DayOfWeek &&
                    ab.StartTime == block.StartTime &&
                    ab.EndTime == block.EndTime)
            );

            var update = Builders<Tenant>.Update.PullFilter(x => x.appointmentSettings.AppointmentsBlocks,
                    ab => ab.DayOfWeek == block.DayOfWeek &&
                    ab.StartTime == block.StartTime &&
                    ab.EndTime == block.EndTime);

            if (await DeleteBlockSlots(dbName, block))
            {
                if (await UpdateCollection(dbName, filter, update, Constants.detailsCollectionName, "DeleteDayOffByDate"))
                {
                    _logger.LogInformation($"Delete available block finished successfully");
                    return true;
                }
            }
            _logger.LogError($"Failed to Delete block {block.ToString()}");
            return false;
        }

        /// <summary>
        /// This function returns all the Appointments Available Blocks for a specific tenant.
        /// </summary>
        public async Task<List<AppointmentsAvailableBlock>?> GetAllAvailableBlocks(string dbName)
        {
            try
            {
                var detailsCollection = _databaseServices.FindCollectionByDB<Tenant>(dbName, Constants.detailsCollectionName);
                var tenant = await detailsCollection.Find(d => d.Id == dbName).FirstOrDefaultAsync();
                if (tenant.appointmentSettings != null)
                {
                    return tenant.appointmentSettings.AppointmentsBlocks.OrderBy(block => block.DayOfWeek).ThenBy(block => block.StartTime).ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Execption! GetAllAvailableBlocks throws an exception for {dbName}. Failed to get all available block types." +
                    $"Exception info: {ex}");
                return null;
            }
        }

        /// <summary>
        /// The function returns the slot duration.
        /// </summary>
        public async Task<TimeSpan?> GetSlotDuration(string dbName)
        {
            try
            {
                var detailsCollection = _databaseServices.FindCollectionByDB<Tenant>(dbName, Constants.detailsCollectionName);
                var tenant = await detailsCollection.Find(d => d.Id == dbName).FirstOrDefaultAsync();
                if (tenant.appointmentSettings != null)
                {
                    return tenant.appointmentSettings.SlotDuration;
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Execption! GetSlotDuration throws an exception for {dbName}. Failed to get slot duration." +
                    $"Exception info: {ex}");
                return null;
            }
        }

        /// <summary>
        /// This function updates slot duration for a specific tenant.
        /// </summary>
        public async Task<bool?> UpdateSlotDuration(string dbName, TimeSpan duration)
        {
            var blocks = await GetAllAvailableBlocks(dbName);
            if (blocks == null)
            {
                _logger.LogError("Failed to update slot duration. There are available blocks.");
                return null;
            }

            var filter = Builders<Tenant>.Filter.And(
             Builders<Tenant>.Filter.Eq(x => x.Id, dbName));

            var update = Builders<Tenant>.Update
               .Set($"{nameof(Tenant.appointmentSettings)}.{nameof(AppointmentSettings.SlotDuration)}", duration);

            // if the update fails, return null.
            var newTimeSlot = await UpdateCollection(dbName, filter, update, Constants.detailsCollectionName, "UpdateSlotDuration");
            if (newTimeSlot == false)
            {
                _logger.LogError($"Failed to update slot duration. Failed to update slot duration for tenant {dbName}");
                return null;
            }

            // if the update succeeded, delete all the slots and generate new ones.
            foreach (var block in blocks)
            {
                if (!await DeleteBlockSlots(dbName, block))
                {
                    _logger.LogError($"Failed to update slot duration. Failed to delete block {block.ToString()}");
                    return null;
                }

                // TODO: change the 14 days to be a const.
                var newSlots = await GenerateSlots(dbName, DateTime.Today, DateTime.Today.AddMonths(Constants.availableBlockMothsDuration), block);
                if (newSlots == null)
                {
                    _logger.LogError($"Failed to update slot duration. Failed to generate slots for block {block.ToString()}");
                    return null;
                }
            }

            return true;

        }


        /// <summary>
        /// The function generates slots for spesific tenant in a spesific time.
        /// </summary>
        public async Task<Dictionary<DateOnly, List<AppointmentSlot>>?> GenerateSlots(string tenantId, DateTime start, DateTime end, AppointmentsAvailableBlock? appointmentBlock = null, TimeSpan? slotDuration = null)
        {
            // Getting appointment settings and validation.
            List<AppointmentsAvailableBlock>? appointmentsAvailableBlocks = new List<AppointmentsAvailableBlock>();
            if (appointmentBlock == null)
            {
                appointmentsAvailableBlocks = await GetAllAvailableBlocks(tenantId);
            }
            else
            {
                appointmentsAvailableBlocks.Add(appointmentBlock);
            }

            if (slotDuration == null)
            {
                slotDuration = await GetSlotDuration(tenantId);
                if (slotDuration == null)
                {
                    _logger.LogError($"Failed to load {tenantId} slot duration");
                    return null;
                }
            }

            if (appointmentsAvailableBlocks == null || slotDuration == TimeSpan.Zero)
            {
                _logger.LogError($"Failed to load {tenantId} appointmens settings");
                return null;
            }

            List<AppointmentsAvailableBlock>[] availableBlockByDay = GetAvailableBlocksPerDay(appointmentsAvailableBlocks);

            // Find the number of days. If the start and end are the same day, we want to generate slots for that day.
            var days = (end.Date - start.Date).TotalDays;
            if (days == 0)
            {
                days = 1;
            }

            List<Holiday>? daysoff = await GetAllDaysOff(tenantId);
            if (daysoff == null)
            {
                _logger.LogError($"Failed to load {tenantId} days off");
                return null;
            }

            var map = await CreateSlotList(tenantId, (int)days, start, daysoff, availableBlockByDay, slotDuration);
            return map;

        }

        /// <summary> create a map with the date as the key and the list of slots as the value. </summary>
        public async Task<Dictionary<DateOnly, List<AppointmentSlot>>> CreateSlotList(string tenantId, int days, DateTime start, List<Holiday> daysoff, List<AppointmentsAvailableBlock>[] availableBlockByDay, TimeSpan? slotDuration)
        {
            Dictionary<DateOnly, List<AppointmentSlot>> map = new Dictionary<DateOnly, List<AppointmentSlot>>();
            for (var day = 0; day < days; day++)
            {
                var currentDay = start.Date.AddDays(day);
                var currentWeekDay = currentDay.DayOfWeek;
                DateOnly tempDate = new DateOnly(currentDay.Year, currentDay.Month, currentDay.Day);
                if (!daysoff.Any(holiday => holiday.Date == tempDate))
                {
                    List<AppointmentSlot> temp = new List<AppointmentSlot>();
                    foreach (var block in availableBlockByDay[(int)currentWeekDay])
                    {
                        List<AppointmentSlot>? availableSlotsForBlock = await CreateAvailableSlotsForAvailableBlock(tenantId, block, currentDay, slotDuration);
                        if (availableSlotsForBlock != null)
                        {
                            temp.AddRange(availableSlotsForBlock);
                        }
                    }
                    // Add appointments slots list to the map with the current date.
                    temp.Sort((a, b) => a.StartTime.CompareTo(b.StartTime));
                    map.Add(tempDate, new List<AppointmentSlot>(temp));
                    temp.Clear();
                }
            }

            _logger.LogInformation("Finish creating available slots");
            await SaveAppointmentsSlotsInDB(tenantId, map);
            return map;
        }


        private async Task<bool> SaveAppointmentsSlotsInDB(string tenantId, Dictionary<DateOnly, List<AppointmentSlot>> map)
        {
            var collectionName = Constants.appointmentsCollectionName;
            var collection = _databaseServices.FindCollectionByDB<BsonDocument>(tenantId, collectionName);
            var existingBsonDocuments = await collection.Find(_ => true).ToListAsync();
            if (collection != null)
            {
                List<BsonDocument> documents = new List<BsonDocument>();
                foreach (var kvp in map)
                {
                    var array = new BsonArray(kvp.Value.Select(slot => slot.ToBsonDocument()));
                    if (array.Count > 0)
                    {
                        BsonDocument document = new BsonDocument();
                        document.Add("date", kvp.Key.ToString());
                        document.Add("slots", array);
                        documents.Add(document);
                    }
                }
                try
                {
                    var combinedDocuments = await RemoveDuplicateDates(existingBsonDocuments, documents, collection);
                    //_logger.LogInformation($"Saving {documents.Count} documents to {collectionName} collection");
                    await collection.InsertManyAsync(combinedDocuments);
                    _logger.LogInformation($"Saved {documents.Count} documents to {collectionName} collection");
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error saving documents to {collectionName} collection: {ex.Message}");
                    return false;
                }
            }
            else
            {
                _logger.LogError("Failed to find collection in DB");
                return false;
            }
        }

        private async Task<List<BsonDocument>> RemoveDuplicateDates(List<BsonDocument> existingDocuments, List<BsonDocument> documents, IMongoCollection<BsonDocument> collection)
        {
            if (existingDocuments == null || existingDocuments.Count == 0)
            {
                return documents;
            }

            // Separate to group - documents with same date.
            var existingDates = existingDocuments.Select(doc => doc["date"]).ToHashSet();
            var newDates = documents.Select(doc => doc["date"]).ToHashSet();
            var commonDates = existingDates.Intersect(newDates);


            var groups = existingDocuments.Concat(documents)
                                          .Where(doc => commonDates.Contains(doc["date"]))
                                          .GroupBy(doc => doc["date"]);

            var newDocuments = documents.Where(doc => !existingDates.Contains(doc["date"].AsString)).ToList();

            List<BsonDocument> combinedDocuments = new List<BsonDocument>();

            // iterate through the groups
            foreach (var group in groups)
            {
                var date = group.Key;
                var matchingDocs = group.ToList();

                if (matchingDocs.Count >= 2)
                {
                    var combinedSlots = new List<BsonValue>();
                    foreach (var doc in matchingDocs)
                    {
                        var slots = doc["slots"].AsBsonArray;
                        combinedSlots.AddRange(slots);
                    }

                    var combinedDoc = new BsonDocument
                    {
                        { "date", date.ToString() },
                        { "slots", new BsonArray(combinedSlots) }
                    };

                    combinedDocuments.Add(combinedDoc);
                    var update = Builders<BsonDocument>.Filter.Eq("date", matchingDocs[0]["date"]);
                    await collection.ReplaceOneAsync(update, combinedDoc);
                }
            }

            // return the non-duplicate documents
            return newDocuments.ToList();
        }


        /// <summary>
        /// The function receives a block of time and slot duration and returns a list of available slots for this time.
        /// </summary>
        private async Task<List<AppointmentSlot>?> CreateAvailableSlotsForAvailableBlock(string tenantId, AppointmentsAvailableBlock block, DateTime currentDay, TimeSpan? slotDuration)
        {
            TimeOnly startTime = new TimeOnly(block.StartTime.Hour, block.StartTime.Minute, block.StartTime.Second);
            TimeOnly endTime = new TimeOnly(block.EndTime.Hour, block.EndTime.Minute, block.EndTime.Second);

            TimeOnly current = startTime;

            List<AppointmentSlot> slots = new List<AppointmentSlot>();
            var appointmentsCollection = _databaseServices.FindCollectionByDB<BsonDocument>(tenantId, Constants.appointmentsCollectionName);
            if (appointmentsCollection == null || slotDuration == null)
            {
                _logger.LogError("Failed to find collection in DB");
                return null;
            }

            while (current.Add(slotDuration.Value) <= endTime)
            {
                if (slotDuration != null)
                {
                    if (await FindSlotOverlapByDateAndTime(appointmentsCollection, DateOnly.FromDateTime(currentDay), current, current.Add(slotDuration.Value)) == false)
                    {
                        // Adding new slot to the list.
                        slots.Add(new AppointmentSlot(
                            startTime: current,
                            endTime: current.Add(slotDuration.Value),
                            dayOfWeek: currentDay.DayOfWeek,
                            status: AppointmentStatus.Available
                        ));

                    }
                    current = current.Add(slotDuration.Value);
                }
                else
                {
                    _logger.LogError("Slot duration is null");
                    return null;
                }
            }

            _logger.LogInformation("Finish creating available slots for available blocks");
            return slots;


        }

        /// <summary>
        /// The function receives a list with all the appointment available blocks.
        /// The function creates another list, in which each cell is a list of AppointmentsAvailableBlock.
        /// Each cell represents a day in the week. The function divides all the blocks for the correct day.
        /// </summary>
        private List<AppointmentsAvailableBlock>[] GetAvailableBlocksPerDay(List<AppointmentsAvailableBlock> availableBlocks)
        {
            // create and initialize an empty list for each day.
            List<AppointmentsAvailableBlock>[] blocksArray = new List<AppointmentsAvailableBlock>[7];
            for (int i = 0; i < blocksArray.Length; i++)
            {
                blocksArray[i] = new List<AppointmentsAvailableBlock>();
            }

            // Adding blocks per day.
            foreach (var block in availableBlocks)
            {
                blocksArray[(int)block.DayOfWeek].Add(block);
            }

            return blocksArray;
        }


        public async Task<bool> ValidateAvailableBlock(string tenantId, AppointmentsAvailableBlock newBlock)
        {
            var appointmentCollection = _databaseServices.FindCollectionByDB<BsonDocument>(tenantId, Constants.appointmentsCollectionName);
            if (appointmentCollection != null)
            {
                var duration = await GetSlotDuration(tenantId);
                TimeSpan timeDifference = TimeSpan.FromTicks(newBlock.EndTime.Ticks - newBlock.StartTime.Ticks);
                if (duration != null && timeDifference < duration)
                {
                    _logger.LogError($"Trying to insert block that is smaller than the block slot duration");
                    return false;
                }
                List<AppointmentsAvailableBlock>? l = await GetAllAvailableBlocks(tenantId);
                if (l != null)
                {
                    List<AppointmentsAvailableBlock>[] array = GetAvailableBlocksPerDay(l);
                    List<AppointmentsAvailableBlock> list = array[(int)newBlock.DayOfWeek];
                    foreach (var block in list)
                    {
                        if (ValidateBlockHours(block, newBlock) == false)
                        {
                            _logger.LogError($"Failed to create new block, {newBlock.ToString()} is overlap with another block.");
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        private bool ValidateBlockHours(AppointmentsAvailableBlock block, AppointmentsAvailableBlock newBlock)
        {
            bool overlap = block.StartTime < newBlock.EndTime && newBlock.StartTime < block.EndTime;
            bool equal = block.StartTime == newBlock.StartTime && newBlock.EndTime == block.EndTime;
            if (overlap || equal)
            {
                return false;
            }
            return true;
        }

        private async Task<bool> DeleteBlockSlots(string tenantId, AppointmentsAvailableBlock block)
        {
            // Find the collection in the tenant DB.
            var appointmentCollection = _databaseServices.FindCollectionByDB<BsonDocument>(tenantId, Constants.appointmentsCollectionName);
            if (appointmentCollection != null)
            {
                var day = (int)block.DayOfWeek;
                var filter = Builders<BsonDocument>.Filter.And(
                     Builders<BsonDocument>.Filter.ElemMatch("slots", Builders<BsonDocument>.Filter.And(
                     Builders<BsonDocument>.Filter.Eq("DayOfWeek", day),
                     Builders<BsonDocument>.Filter.Gte("StartTime", block.StartTime),
                     Builders<BsonDocument>.Filter.Lte("EndTime", block.EndTime),
                     Builders<BsonDocument>.Filter.Eq("Status", AppointmentStatus.Available.ToString()))));

                // Find documents and remove filtered slots.
                var documents = await appointmentCollection.Find(filter).ToListAsync();
                List<AppointmentSlot> temp = new List<AppointmentSlot>();
                Dictionary<DateOnly, List<AppointmentSlot>> map = new Dictionary<DateOnly, List<AppointmentSlot>>();
                foreach (var doc in documents)
                {
                    var slots = doc["slots"].AsBsonArray;
                    foreach (var slot in slots)
                    {
                        //var currentSlot = AppointmentSlot.CreateSlotFromBsonDocument(doc, i);
                        var currentSlot = BsonSerializer.Deserialize<AppointmentSlot>(slot.AsBsonDocument);
                        if (block.StartTime < currentSlot.EndTime && currentSlot.StartTime < block.EndTime)
                        {
                            if (currentSlot.Status != AppointmentStatus.Available.ToString())
                            {
                                temp.Add(currentSlot);
                            }
                        }
                        else
                        {
                            temp.Add(currentSlot);
                        }
                    }
                    DateOnly dateOnly = Utils.ExtractDateFromBsonDocument(doc);
                    map.Add(dateOnly, new List<AppointmentSlot>(temp));
                    temp.Clear();
                }

                // Replacing each document with new document. In case there are not slots, delete the document.
                try
                {
                    var newDocuments = ConvertListToBsonDocuments(map);
                    foreach (var doc in newDocuments)
                    {
                        filter = Builders<BsonDocument>.Filter.Eq("date", doc["date"]);
                        if (doc["slots"].AsBsonArray.Count > 0)
                        {
                            await appointmentCollection.ReplaceOneAsync(filter, doc);
                        }
                        else
                        {
                            await appointmentCollection.DeleteOneAsync(filter);
                        }
                    }
                }
                catch
                {
                    _logger.LogError($"Did not find slots {block.ToString()}");
                    return false;
                }

                _logger.LogInformation($"{block.ToString()} deleted for {tenantId}");
                
                return true;
            }
            _logger.LogError("Faild to find collection in DB");
            return false;
        }

        /// <summary>
        /// The function receives a map with dates and slots and return it as bson documents.
        /// </summary>
        private List<BsonDocument> ConvertListToBsonDocuments(Dictionary<DateOnly, List<AppointmentSlot>> map)
        {
            List<BsonDocument> documents = new List<BsonDocument>();
            foreach (var kvp in map)
            {
                var array = new BsonArray(kvp.Value.Select(slot => slot.ToBsonDocument()));
                BsonDocument document = new BsonDocument();
                document.Add("date", kvp.Key.ToString());
                document.Add("slots", array);
                documents.Add(document);
            }

            return documents;
        }


        public async Task<bool> FindSlotOverlapByDateAndTime(IMongoCollection<BsonDocument> appointmentCollection, DateOnly date, TimeOnly StartTime, TimeOnly EndTime)
        {

            // find dayofweek by date.
            var day = (int)date.DayOfWeek;

            // Find documents and remove filtered slots, in the spesific date.
            var filter = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq("date", date.ToString()),
            Builders<BsonDocument>.Filter.ElemMatch("slots", Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq("DayOfWeek", day),
            Builders<BsonDocument>.Filter.Lt("StartTime", EndTime),
            Builders<BsonDocument>.Filter.Gt("EndTime", StartTime))));

            var documents = await appointmentCollection.Find(filter).ToListAsync();
            if (documents.Count > 0)
            {
                return true;
            }

            return false;
        }


        private async Task<bool> DeleteSlotsByDate(string tenantId, DateOnly date)
        {
            // Find the collection in the tenant DB.
            var appointmentCollection = _databaseServices.FindCollectionByDB<BsonDocument>(tenantId, Constants.appointmentsCollectionName);
            if (appointmentCollection != null)
            {
                var filter = Builders<BsonDocument>.Filter.Eq("date", date.ToString());
                var res = await appointmentCollection.DeleteOneAsync(filter);

                // verify delete was successful.
                if (res.DeletedCount == 1)
                {
                    _logger.LogInformation($"Deleted all slots for {date.ToString()} in {tenantId}");
                    return true;
                }
            }

            _logger.LogError($"Failed to delete all slots for {date.ToString()} in {tenantId}");
            return false;

        }


    }
}
