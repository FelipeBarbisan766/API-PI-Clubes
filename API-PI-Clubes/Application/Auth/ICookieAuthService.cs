using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Auth;

public interface ICookieAuthService
{
    Task SignInAsync(HttpContext httpContext, User user);
    Task SignOutAsync(HttpContext httpContext);
}