﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OMSAPI.Dto.EntitiesDto
{
    public class TenantDto
    {
        //public string Id { get; set; } = string.Empty;

        [Required]
        [MinLength(2, ErrorMessage = "Name must be at least 2 characters long.")]
        [RegularExpression("^[a-zA-Z][a-zA-Z0-9]*$", ErrorMessage = "Name must contain at least one letter and can only contain letters and numbers.")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [Required]
        [JsonPropertyName("password")]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d).{5,}$", ErrorMessage = "Password must contain at least one letter and one digit, and be at least 5 characters long.")]
        public string Password { get; set; }

        [Required]
        [JsonPropertyName("email")]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [JsonPropertyName("phoneNumber")]

        public string PhoneNumber { get; set; } = string.Empty;

        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;
    }
}