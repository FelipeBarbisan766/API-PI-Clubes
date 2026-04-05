namespace API_PI_Clubes.Infrastructure.Settings
{
    public class JwtSettings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int Expiration { get; set; }
        public string EmailVerificationKey { get; set; }
        public int ExpirationEmailVerification { get; set; }
        public string ResetPasswordKey { get; set; }
        public int ExpirationResetPassword { get; set; }
    }
}
