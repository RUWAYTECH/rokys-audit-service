using Microsoft.Extensions.Options;
using Rokys.Audit.DTOs.Requests.Email;
using System.Net.Mail;
using System.Net;

namespace Rokys.Audit.External.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendEmailAsync(
            IEnumerable<string> toList,
            string subject,
            string body,
            bool isHtml = true)
        {
            using var client = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort)
            {
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                EnableSsl = true
            };

            // Configurar el remitente con nombre y email
            var fromEmail = !string.IsNullOrEmpty(_settings.FromEmail) ? _settings.FromEmail : _settings.Username;
            var fromAddress = new MailAddress(fromEmail, _settings.FromName ?? "Notificación Rokys");

            using var mail = new MailMessage
            {
                From = fromAddress,
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            // Agregar todos los destinatarios
            foreach (var to in toList)
            {
                if (!string.IsNullOrWhiteSpace(to))
                    mail.To.Add(new MailAddress(to.Trim()));
            }

            await client.SendMailAsync(mail);
        }
    }
}
