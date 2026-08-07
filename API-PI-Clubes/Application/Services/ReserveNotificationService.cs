using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Hubs;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace API_PI_Clubes.Application.Services
{
    public class ReserveNotificationService : IReserveNotificationService
    {
        private readonly IHubContext<CourtAvailabilityHub, ICourtAvailabilityHubClient> _hubContext;

        public ReserveNotificationService(
            IHubContext<CourtAvailabilityHub, ICourtAvailabilityHubClient> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyStatusChangedAsync(Guid clubId, ReserveAvailabilityChangedDTO dto)
        {
            await _hubContext.Clients.Group($"club-{clubId}").ReserveStatusChanged(dto);
        }
    }
}