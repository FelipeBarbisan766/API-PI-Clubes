
using System.Security.Claims;
using API_PI_Clubes.Application.Auth;
using API_PI_Clubes.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

[ApiController]
[Route("api/backoffice/auth")]
public class BackofficeAuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public BackofficeAuthController(IAuthService authService) => _authService = authService;

    [HttpPost("google-login")]
    [EnableRateLimiting(BackofficeAuth.LoginRateLimit)]
    public async Task<IActionResult> GoogleLogin(
        [FromBody] BackofficeGoogleLoginDTO dto, CancellationToken cancellationToken)
    {
        var user = await _authService.BackofficeGoogleLogin(dto.IdToken, cancellationToken);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, BackofficeAuth.Scheme));
        await HttpContext.SignInAsync(BackofficeAuth.Scheme, principal);

        return Ok(new { user.Id, user.Name, user.Email, user.Role });
    }

    [HttpPost("logout")]
    [Authorize(Policy = BackofficeAuth.Policy)]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(BackofficeAuth.Scheme);
        return NoContent();
    }

    // O Angular usa para saber se a sessão do painel ainda é válida
    [HttpGet("me")]
    [Authorize(Policy = BackofficeAuth.Policy)]
    public IActionResult Me() => Ok(new
    {
        Id = User.GetUserId(),
        Name = User.Identity?.Name
    });
}