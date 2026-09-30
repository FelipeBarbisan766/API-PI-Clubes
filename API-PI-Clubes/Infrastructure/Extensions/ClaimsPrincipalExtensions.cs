using System.Security.Claims;
using API_PI_Clubes.Application.Exceptions;

namespace API_PI_Clubes.Infrastructure.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var id))
            throw new NotFoundException("User ID not found in token");

        return id;
    }
}