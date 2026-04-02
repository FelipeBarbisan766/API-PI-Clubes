using API_PI_Clubes.Application.Email;
using Microsoft.Extensions.Options;
using System.Net.Mail;

public class EmailSettings
{
    public string SenderEmail { get; set; }
    public string SenderName { get; set; }
    public string SmtpServer { get; set; }
    public int SmtpPort { get; set; }
    public string SmtpUsername { get; set; }
    public string SmtpPassword { get; set; }
    public bool EnableSsl { get; set; }
}
