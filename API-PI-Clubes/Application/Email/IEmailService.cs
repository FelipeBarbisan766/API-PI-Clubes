namespace API_PI_Clubes.Application.Email
{
    public interface IEmailService
    {
        Task SendVerificationEmailAsync(string recipientEmail, string recipientName, string jwtToken);
        Task SendResetPasswordAsync(string recipientEmail, string recipientName, string jwtToken);
    }
}
