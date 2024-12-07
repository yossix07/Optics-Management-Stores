using MailKit.Net.Smtp;
using MimeKit;
using OMSAPI.Dto.AppointmentsDto;
using OMSAPI.Dto.StoreDto;
using OMSAPI.Models.Entities;
using OMSAPI.Services.ServicesInterfaces;
using Razor.Templating.Core;

namespace OMSAPI.Services.MailServices
{
    public class EmailServices : IEmailServices
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailServices> _logger;

        //private readonly static string messageBody = "<i> This is a temporary message </i>";

        public EmailServices(IConfiguration configuration, ILogger<EmailServices> logger)
        {
            _config = configuration;
            _logger = logger;
        }

        public bool SendEmail(string emailAddress, string subject, string body)
        {
            try
            {
                var host = _config.GetSection("EmailSettings:EmailHost").Value;
                var username = _config.GetSection("EmailSettings:EmailUserName").Value;
                var password = _config.GetSection("EmailSettings:EmailPassword").Value;

                // Create email message as do not reply email address
                // question: how to define the email address as do not reply? 


                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("Mailbox", username));
                email.To.Add(MailboxAddress.Parse(emailAddress));
                email.Subject = subject;

                
                // Render the HTML bill template and set it as the email body
                email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

                var smtp = new SmtpClient();
                smtp.Connect(host, 465, true);
                smtp.AuthenticationMechanisms.Remove("XOAUTH2");
                smtp.Authenticate(username, password);
                smtp.Send(email);
                smtp.Disconnect(true);
                smtp.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email. Exception: {ex}");
                return false;
            }
        }

        public async Task<bool> SendUserOrderEmail(string emailAddress, OrderEmailReponseDto respose)
        {
            // Load email template with model
            var body = await RazorTemplateEngine.RenderAsync("/Views/UserOrderConfirmationEmail.cshtml", respose);
            var subject = "Order confirmation";
            return SendEmail(emailAddress,subject, body);
        }

        public async Task<bool> SendTenantOrderEmail(string emailAddress, OrderEmailReponseDto respose)
        {
            // Load email template with model
            var body = await RazorTemplateEngine.RenderAsync("/Views/TenantOrderConfirmationEmail.cshtml", respose);
            var subject = "New order completed";
            return SendEmail(emailAddress, subject, body);
        }

        public bool SendResetPasswordEmail(string emailAddress, string verificationCode)
        {
            var body = $"<h1> Your verification code code is: {verificationCode} </h1>";
            var subject = "Reset Password";
            return SendEmail(emailAddress, subject, body);
        }

        public async Task<bool> NotifyUserAboutAppointmentCreation(string emailAddress, Dictionary<DateOnly, AppointmentSlotDto> dto, Tenant tenant, User user)
        {
            AppointmentEmailDto emailDto = new AppointmentEmailDto(
            dto.Keys.First(), dto.Values.First(), tenant, user);
            var body = await RazorTemplateEngine.RenderAsync("/Views/UserAppointmentCreation.cshtml", emailDto);
            var subject = "Appointment Confirmation";
            return SendEmail(emailAddress, subject, body);
        }

        public async Task<bool> NotifyTenantAboutAppointmentCreation(string emailAddress, Dictionary<DateOnly, AppointmentSlotDto> dto, Tenant tenant, User user)
        {
            AppointmentEmailDto emailDto = new AppointmentEmailDto(
                dto.Keys.First(), dto.Values.First(), tenant, user);
            var body = await RazorTemplateEngine.RenderAsync("/Views/TenantAppointmentCreation.cshtml", emailDto);
            var subject = "Appointment Confirmation";
            return SendEmail(emailAddress, subject, body);
        }


        public async Task<bool> NotifyUserAboutAppointmentCancelation(string emailAddress, Dictionary<DateOnly, AppointmentSlotDto> dto, Tenant tenant, User user)
        {
            AppointmentEmailDto emailDto = new AppointmentEmailDto(
                dto.Keys.First(), dto.Values.First(), tenant, user);
            var body = await RazorTemplateEngine.RenderAsync("/Views/UserAppointmentCancellation.cshtml", emailDto);
            var subject = "Appointment Cancelation";
            return SendEmail(emailAddress, subject, body);
        }

        public async Task<bool> NotifyTenantAboutAppointmentCancelation(string emailAddress, Dictionary<DateOnly, AppointmentSlotDto> dto, Tenant tenant, User user)
        {
            AppointmentEmailDto emailDto = new AppointmentEmailDto(
                dto.Keys.First(), dto.Values.First(), tenant, user);
            var body = await RazorTemplateEngine.RenderAsync("/Views/TenantAppointmentCancellation.cshtml", emailDto);
            var subject = "Appointment Cancelation";
            return SendEmail(emailAddress, subject, body);
        }
    }
}
