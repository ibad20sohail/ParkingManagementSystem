using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PMS.Application.IServices;
using PMS.Application.Settings;
using System.Net;
using System.Net.Mail;

namespace PMS.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly IHostEnvironment _environment;
        public EmailService(IOptions<ApplicationParameters> options, IHostEnvironment environment)
        {
            _settings = options.Value.EmailSettings;
            _environment = environment;
        }

        public async Task<string> GetTemplateAsync(string templateName, Dictionary<string, string> placeholders)
        {
            var path = Path.Combine(_environment.ContentRootPath, "../", "PMS.Application", "Templates", "Email", $"{templateName}.html");

            var html = await File.ReadAllTextAsync(path);

            foreach (var placeholder in placeholders)
            {
                html = html.Replace(
                    $"{{{{{placeholder.Key}}}}}",
                    placeholder.Value);
            }

            return html;
        }
        public async Task SendAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default)
        {
            if (!_settings.EnableEmailSend)
                return;

            using var message = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail, _settings.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            message.To.Add(to);

            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                EnableSsl = _settings.EnableSsl,
                Credentials = new NetworkCredential(_settings.Username, _settings.Password)
            };

            await client.SendMailAsync(message, cancellationToken);
        }
    }
}
