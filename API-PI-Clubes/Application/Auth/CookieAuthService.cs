// Infrastructure/Security/CookieAuthService.cs
using API_PI_Clubes.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using API_PI_Clubes.Application.Auth;

namespace API_PI_Clubes.Infrastructure.Security
{
    public class CookieAuthService : ICookieAuthService
    {
        public async Task SignInAsync(HttpContext httpContext, User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.Email, user.Email)
            };

            foreach (var role in user.Role.ToString().Split(','))
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Trim()));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true 
                });
        }

        public Task SignOutAsync(HttpContext httpContext) =>
            httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}