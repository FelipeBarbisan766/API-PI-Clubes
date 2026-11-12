using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IRepositories;

public interface IRootRepository
{
    Task<OverviewRootDTO> GetOverviewAsync(CancellationToken cancellationToken);
}