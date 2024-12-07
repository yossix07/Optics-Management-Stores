using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using OMSAPI.Models.Entities;
using System.Security.Claims;

namespace OMSAPI.General
{
    public class Utils
    {
        public static DateOnly ExtractDateFromBsonDocument(BsonDocument doc)
        {
            // Extract date from document. 
            var dateTime = DateTime.Parse(doc["date"].AsString);
            return DateOnly.FromDateTime(dateTime.ToUniversalTime());

        }

        
        public static ActionResult LogErrorAndReturnBadRequest(ILogger logger, string error)
        {
            logger.LogError(error);
            return new BadRequestObjectResult(error);
        }
        

    }
}
