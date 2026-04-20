using System.Threading.Tasks;

namespace backend.context.notification.application.services;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);
}
