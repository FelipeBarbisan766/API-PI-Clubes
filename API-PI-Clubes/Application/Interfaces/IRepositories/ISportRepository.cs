using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Interfaces.IRepositories
{
    public interface ISportRepository
    {
        Task<int> CountExistingAsync(List<Guid> ids,CancellationToken cancellationToken);
        Task<List<ResponseSportDTO>> GetAllAsync(CancellationToken cancellationToken);
        Task<List<ResponseSportDTO>> GetByIdsAsync(List<Guid> ids, CancellationToken cancellationToken);
    }
}