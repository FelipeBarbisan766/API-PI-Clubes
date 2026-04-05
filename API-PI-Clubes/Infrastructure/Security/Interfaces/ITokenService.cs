using API_PI_Clubes.Model;
using System.Security.Claims;

namespace API_PI_Clubes.Infrastructure.Security.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        string GenerateEmailVerificationToken(Guid id);
        ClaimsPrincipal? ValidateEmailVerificationToken(string token);
        string GenerateEmailResetPasswordToken(Guid id);
        ClaimsPrincipal? ValidateEmailResetPasswordToken(string token);

    }
}
