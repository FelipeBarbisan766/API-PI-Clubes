using API_PI_Clubes.Application.Auth;
using API_PI_Clubes.Application.Email;
using API_PI_Clubes.Application.Interfaces.IMappers;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Application.Mappers;
using API_PI_Clubes.Application.Services;
using API_PI_Clubes.Application.Storage;
using API_PI_Clubes.Infrastructure.Repositories;
using API_PI_Clubes.Infrastructure.Security;
using API_PI_Clubes.Infrastructure.Security.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace API_PI_Clubes.Application.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IClubService, ClubService>();
            services.AddScoped<IClubRepository, ClubRepository>();
            services.AddScoped<IClubMapper, ClubMapper>();

            services.AddScoped<ICourtService, CourtService>();
            services.AddScoped<ICourtRepository, CourtRepository>();
            services.AddScoped<ICourtMapper, CourtMapper>();
            
            services.AddScoped<IReserveService, ReserveService>();
            services.AddScoped<IReserveRepository, ReserveRepository>();
            services.AddScoped<IReserveMapper, ReserveMapper>();
            
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IScheduleRepository, ScheduleRepository>();
            services.AddScoped<IScheduleMapper, ScheduleMapper>();
            
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserMapper, UserMapper>();
            
            services.AddScoped<IPlayerService, PlayerService>();
            services.AddScoped<IPlayerRepository, PlayerRepository>();
            services.AddScoped<IPlayerMapper, PlayerMapper>();
            
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<IAdminMapper, AdminMapper>();
            
            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<EmailBodyService>();
            services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<IStorageService, AzureStorageService>();

            services.AddScoped<ITokenService, TokenService>();

            services.AddScoped<IPasswordHasher, PasswordHasher>();

            return services;
        }
    }
}
