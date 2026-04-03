using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Auth
{
    public interface IAuthService
    {
        Task<string> LoginAsync(LoginDTO dto);
        Task Register(CreatUserDTO dto);
        Task<bool> ValidateEmailToken(string token);
        Task<bool> ResendEmailToken(string email);

    }
}
