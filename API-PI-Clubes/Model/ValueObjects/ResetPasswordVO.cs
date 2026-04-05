using Microsoft.Extensions.Primitives;

namespace API_PI_Clubes.Model.ValueObjects
{
    public class ResetPasswordVO
    {
        public string? PasswordResetToken { get; private set; }
        public DateTime? ResetTokenExpires { get; set; }
        public DateTime? ResetPasswordAt { get; private set; }

        private ResetPasswordVO(string passwordResetToken, DateTime? resetTokenExpires, DateTime? resetPasswordAt)
        {
            PasswordResetToken = passwordResetToken;
            ResetTokenExpires = resetTokenExpires;
            ResetPasswordAt = resetPasswordAt;
        }

        public static ResetPasswordVO Create(String token,DateTime time)
            => new ResetPasswordVO(token,time, null);

        public static ResetPasswordVO Confirm()
            => new ResetPasswordVO(null, DateTime.UtcNow, DateTime.UtcNow);
    }
}
