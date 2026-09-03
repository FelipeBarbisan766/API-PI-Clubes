namespace API_PI_Clubes.Application.Auth
{
    public class AuthDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class VerifyToken
    {
        public string Token { get; set; }
    }
    public class ResetPassword
    {
        public string Token { get; set; }
        public string Password { get; set; }
    }
}
