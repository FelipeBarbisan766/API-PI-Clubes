using API_PI_Clubes.Application.Auth;
using API_PI_Clubes.Application.Interfaces;
using API_PI_Clubes.Application.Services;
using API_PI_Clubes.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;

namespace API_PI_Clubes.Application.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ICourtService, CourtService>();
            services.AddScoped<IReserveService, ReserveService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPlayerService, PlayerService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            return services;
        }
    }
}
