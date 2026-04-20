using System.Threading.Tasks;
using backend.context.notification.application.services;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace backend.context.notification.infrastructure.services;

public class MailKitEmailService : IEmailService
{
    private readonly IConfiguration _config;

    public MailKitEmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
    {
        var host = _config["SmtpSettings:Host"];
        var portStr = _config["SmtpSettings:Port"];
        var senderName = _config["SmtpSettings:SenderName"];
        var senderEmail = _config["SmtpSettings:SenderEmail"];
        var appPassword = _config["SmtpSettings:Password"];

        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(appPassword))
            throw new System.Exception("SmtpSettings chưa được cấu hình đúng trong appsettings.json.");

        int port = int.TryParse(portStr, out var p) ? p : 587;

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(senderName, senderEmail));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = isHtml ? body : null,
            TextBody = !isHtml ? body : null
        };
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(senderEmail, appPassword);
        
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
