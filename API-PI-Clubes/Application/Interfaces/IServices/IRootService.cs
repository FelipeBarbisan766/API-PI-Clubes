using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices;

public interface IRootService
{
    Task<OverviewRootDTO> Overview(CancellationToken cancellationToken);
}