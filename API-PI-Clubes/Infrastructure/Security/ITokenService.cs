using API_PI_Clubes.Model;

namespace API_PI_Clubes.Infrastructure.Security
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
