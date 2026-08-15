using System.Security.Claims;

namespace API_PI_Clubes.Infrastructure.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var id))
            throw new Exception("User ID not found in token");

        return id;
    }
}