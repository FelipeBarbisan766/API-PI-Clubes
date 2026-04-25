using System.Security.Claims;
using API_PI_Clubes.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using API_PI_Clubes.Model;
using Microsoft.AspNetCore.Authorization;

namespace API_PI_Clubes.Application.Auth;

public class ClubAuthorizationHandler : AuthorizationHandler<ManageClubRequirement, Guid>
{
    private readonly AppDbContext _context;

    public ClubAuthorizationHandler(AppDbContext context)
    {
        _context = context;
    }
    
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ManageClubRequirement requirement,
        Guid clubId)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim == null)
        {
            return;
        }
        if (!Guid.TryParse(userIdClaim, out Guid userId))
        {
            return;
        }

        bool isAdminOfClub = await _context.ClubAdmins
            .AnyAsync(ca =>
                ca.ClubId == clubId && ca.Admin.UserId == userId);

        if (isAdminOfClub)
        {
            context.Succeed(requirement);
        }

        
    }
}