using Application.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var smtpHost = _configuration["Email:Smtp:Host"];
        var smtpPort = int.Parse(_configuration["Email:Smtp:Port"]!);
        var from = _configuration["Email:Smtp:From"];
        var password = _configuration["Email:Smtp:Password"];

        var message = new MailMessage();
        message.From = new MailAddress(from!);
        message.To.Add(to);
        message.Subject = subject;
        message.Body = body;
        message.IsBodyHtml = true;

        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(from, password),
            EnableSsl = true
        };

        await client.SendMailAsync(message);
    }
}
