namespace Rokys.Audit.External.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(IEnumerable<string> to, string subject, string body, bool isHtml = true);
    }
}
