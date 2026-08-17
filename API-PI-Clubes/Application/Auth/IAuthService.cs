using API_PI_Clubes.Application.DTOs;
using System.Security.Claims;
using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Auth
{
    public interface IAuthService
    {
        Task<User> LoginAsync(LoginDTO dto);       
        Task Register(CreatUserDTO dto);
        Task<bool> ValidateEmailToken(string token);
        Task<bool> ResendEmailToken(string email);
        Task RequestResetPassword(string email);
        Task<bool> ResetPassword(string token, string password);
        Task<UserDTO> GetCurrentUserInfo(Guid id);
        Task GoogleSignUp(string idToken);
        Task<User> GoogleLogin(string idToken); 
        Task CompleteProfile(Guid userId, CompleteProfileDTO dto);
    }
}
