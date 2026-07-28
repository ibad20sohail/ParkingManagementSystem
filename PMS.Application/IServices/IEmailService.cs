namespace PMS.Application.IServices;

public interface IEmailService
{
    Task<string> GetTemplateAsync(string templateName, Dictionary<string, string> placeholders);
    Task SendAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default);
}
