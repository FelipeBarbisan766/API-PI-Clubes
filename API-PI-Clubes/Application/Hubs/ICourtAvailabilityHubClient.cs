using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Hubs;

public interface ICourtAvailabilityHubClient
{
    Task ReserveStatusChanged(ReserveAvailabilityChangedDTO dto);
}