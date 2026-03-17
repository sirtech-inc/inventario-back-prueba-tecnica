using Application.Emails.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Application.Emails.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var host = _config["EmailSettings:SmtpHost"] ?? throw new InvalidOperationException("EmailSettings:SmtpHost no está configurado.");
            var portValue = _config["EmailSettings:SmtpPort"] ?? throw new InvalidOperationException("EmailSettings:SmtpPort no está configurado.");
            var username = _config["EmailSettings:Username"] ?? throw new InvalidOperationException("EmailSettings:Username no está configurado.");
            var password = _config["EmailSettings:Password"] ?? throw new InvalidOperationException("EmailSettings:Password no está configurado.");

            using var client = new SmtpClient(
                host,
                int.Parse(portValue))
            {
                Credentials = new NetworkCredential(
                    username,
                    password),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(username, "Inventario"),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            mail.To.Add(to);

            await client.SendMailAsync(mail);
        }
    }
}

