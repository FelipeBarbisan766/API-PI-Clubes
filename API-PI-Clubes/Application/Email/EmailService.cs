using API_PI_Clubes.Application.Email;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

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
            
            var options = _settings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None;

            await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, options);

            await client.AuthenticateAsync(_settings.SmtpUsername, _settings.SmtpPassword);
            await client.SendAsync(message);

            await client.DisconnectAsync(true);

            _logger.LogInformation("Email enviado com sucesso para {Email}", recipientEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha crítica no envio de email para {Email}. Erro: {Message}", recipientEmail, ex.Message);
            throw; 
        }
    }

    public async Task SendVerificationEmailAsync(string recipientEmail, string recipientName, string jwtToken)
    {
       
        var baseUrl = "http://localhost:5000/api/auth/verify-email";
        var verificationLink = $"{baseUrl}?token={jwtToken}";

        var subject = "Verifique seu Email - Clube PI";
        var htmlBody = _emailBodyService.GenerateVerificationEmailHtml(recipientName, verificationLink);

        await SendEmailAsync(recipientEmail, recipientName, subject, htmlBody);
    }


}