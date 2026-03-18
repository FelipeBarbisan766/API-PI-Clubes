using API_PI_Clubes.Application.Interfaces;
using API_PI_Clubes.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace API_PI_Clubes.Application.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ICourtService, CourtService>();

            return services;
        }
    }
}
