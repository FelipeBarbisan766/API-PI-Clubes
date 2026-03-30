using API_PI_Clubes.Application.Auth;
using API_PI_Clubes.Application.Interfaces.IMappers;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Application.Mappers;
using API_PI_Clubes.Application.Services;
using API_PI_Clubes.Infrastructure.Repositories;
using API_PI_Clubes.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;

namespace API_PI_Clubes.Application.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IClubService, ClubService>();

            services.AddScoped<ICourtService, CourtService>();
            
            services.AddScoped<IReserveService, ReserveService>();
            
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IScheduleRepository, ScheduleRepository>();
            services.AddScoped<IScheduleMapper, ScheduleMapper>();
            
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
            
            services.AddScoped<IPlayerService, PlayerService>();
            
            services.AddScoped<IAdminService, AdminService>();
            
            services.AddScoped<IAuthService, AuthService>();
            
            services.AddScoped<ITokenService, TokenService>();


            services.AddScoped<IPasswordHasher, PasswordHasher>();

            return services;
        }
    }
}
