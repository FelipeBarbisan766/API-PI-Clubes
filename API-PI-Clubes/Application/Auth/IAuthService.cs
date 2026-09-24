using API_PI_Clubes.Application.DTOs;
using System.Security.Claims;
using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Auth
{
    public interface IAuthService
    {
        Task<User> LoginAsync(AuthDTO dto, CancellationToken cancellationToken);       
        Task Register(CreatUserDTO dto, CancellationToken cancellationToken);
        Task<bool> ValidateEmailToken(string token, CancellationToken cancellationToken);
        Task<bool> ResendEmailToken(string email, CancellationToken cancellationToken);
        Task RequestResetPassword(string email, CancellationToken cancellationToken);
        Task<bool> ResetPassword(ResetPassword request, CancellationToken cancellationToken);
        Task ChangePassword(Guid userId, ChangePasswordDTO request, CancellationToken cancellationToken);
        Task<UserDTO> GetCurrentUserInfo(Guid id, CancellationToken cancellationToken);
        Task GoogleSignUp(string idToken, CancellationToken cancellationToken);
        Task<User> GoogleLogin(string idToken, CancellationToken cancellationToken); 
        Task CompleteProfile(Guid userId, CompleteProfileDTO dto, CancellationToken cancellationToken);
    }
}
