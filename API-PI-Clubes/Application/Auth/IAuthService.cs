namespace API_PI_Clubes.Application.Auth
{
    public interface IAuthService
    {
        Task<string> LoginAsync(LoginDTO dto);
    }
}
