using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using API_PI_Clubes.Application.Email;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;
    private readonly EmailBodyService _emailBodyService;

    public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger, EmailBodyService emailBodyService)
    {
        _settings = settings.Value;
        _logger = logger;
        _emailBodyService = emailBodyService;
    }

    private async Task SendEmailAsync(string recipientEmail, string recipientName, string subject, string htmlBody)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        message.To.Add(new MailboxAddress(recipientName, recipientEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, _settings.EnableSsl);
            await client.AuthenticateAsync(_settings.SmtpUsername, _settings.SmtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation($"Email enviado para {recipientEmail}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Falha no envio de email: {ex.Message}");
            throw;
        }
    }

    public async Task SendVerificationEmailAsync(string recipientEmail, string recipientName, string jwtToken)
    {
       
        var baseUrl = "https://seusite.com/verify-email";
        var verificationLink = $"{baseUrl}?token={jwtToken}";

        var subject = "Verifique seu Email";
        var htmlBody = _emailBodyService.GenerateVerificationEmailHtml(recipientName, verificationLink);

        await SendEmailAsync(recipientEmail, recipientName, subject, htmlBody);
    }


}