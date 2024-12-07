using FakeItEasy;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using OMSAPI.Models.Appointments;
using OMSAPI.Models.Entities;
using OMSAPI.Roles;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OMSAPITesting.Utils
{
    public class Utils
    {
        public static HttpContext CreateDefaultHttpContext(Tenant tenant)
        {
            // Create a new default HttpContext
            var httpContext = new DefaultHttpContext();

            // Create a new ClaimsIdentity for the user
            var claimsIdentity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Name, tenant.Id),
                new Claim(ClaimTypes.Role, Roles.Tenant.ToString())
                // Add any other claims for the user as needed
            });

            // Create a new ClaimsPrincipal with the user's ClaimsIdentity
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Set the HttpContext User to the new ClaimsPrincipal
            httpContext.User = claimsPrincipal;

            // Use the JWT token middleware to create a token for the user and add it to the HttpContext
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken());
            httpContext.Request.Headers.Add("Authorization", $"Bearer {jwtToken}");

            return httpContext;
        }

        public static HttpContext CreateDefaultHttpContext(User user)
        {
            // Create a new default HttpContext
            var httpContext = new DefaultHttpContext();

            // Create a new ClaimsIdentity for the user
            var claimsIdentity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id),
                new Claim(ClaimTypes.Role, Roles.User.ToString())
                // Add any other claims for the user as needed
            });

            // Create a new ClaimsPrincipal with the user's ClaimsIdentity
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Set the HttpContext User to the new ClaimsPrincipal
            httpContext.User = claimsPrincipal;

            // Use the JWT token middleware to create a token for the user and add it to the HttpContext
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken());
            httpContext.Request.Headers.Add("Authorization", $"Bearer {jwtToken}");

            return httpContext;
        }

        public static Tenant createCustomAppointmentSettings()
        {
            // Fake tenant and appointmentSettings
            var tenantId = ObjectId.GenerateNewId().ToString();
            var tenant = A.Fake<Tenant>();
            tenant.Id = tenantId;

            var appointmentType = new AppointmentType
            {
                TypeName = "test",
                Price = 10,
            };
            var appointmentSettings = A.Fake<AppointmentSettings>();
            appointmentSettings.AppointmentTypes = new List<AppointmentType> { appointmentType };
            tenant.appointmentSettings = appointmentSettings;

            return tenant;
        }
    }
}
