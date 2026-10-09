using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Text.Encodings.Web;

namespace VStore.Services;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string htmlBody);
    Task SendSupportTicketAsync(SupportTicketViewModel ticket);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string htmlBody)
    {
        try
        {
            var smtpHost = _config["Email:SmtpHost"];
            var smtpPort = int.Parse(_config["Email:SmtpPort"] ?? "587");
            var fromEmail = _config["Email:FromEmail"];
            var fromPassword = _config["Email:FromPassword"];

            if (string.IsNullOrWhiteSpace(smtpHost) || string.IsNullOrWhiteSpace(fromEmail) || string.IsNullOrWhiteSpace(fromPassword))
                throw new InvalidOperationException("SMTP settings are incomplete.");

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(fromEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = htmlBody };

            using var client = new SmtpClient();
            await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(fromEmail, fromPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            throw;
        }
    }

    public Task SendSupportTicketAsync(SupportTicketViewModel ticket)
    {
        var supportEmail = _config["Email:FromEmail"];
        if (string.IsNullOrWhiteSpace(supportEmail))
            throw new InvalidOperationException("Support email is not configured.");

        var encoder = HtmlEncoder.Default;
        var name = ticket.Name.Trim();
        var email = ticket.Email.Trim();
        var subject = ticket.Subject.Trim();
        var message = ticket.Message.Trim();
        var htmlBody = $"""
            <h2>Новое обращение в поддержку V Store</h2>
            <p><strong>Имя:</strong> {encoder.Encode(name)}</p>
            <p><strong>Почта:</strong> {encoder.Encode(email)}</p>
            <p><strong>Тема:</strong> {encoder.Encode(subject)}</p>
            <p><strong>Сообщение:</strong></p>
            <p>{encoder.Encode(message).Replace("\n", "<br />")}</p>
            """;

        return SendAsync(supportEmail, $"[V Store Support] {subject}", htmlBody);
    }
}