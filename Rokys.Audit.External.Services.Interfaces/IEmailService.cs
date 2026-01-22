namespace Rokys.Audit.External.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(IEnumerable<string> to, string subject, string body, bool isHtml = true);
        Task SendEmailWithAttachmentsAsync(IEnumerable<string> to, string subject, string body, IEnumerable<(string fileName, byte[] content)> attachments, bool isHtml = true);
    }
}
