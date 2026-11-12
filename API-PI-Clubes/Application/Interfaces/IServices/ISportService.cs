using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface ISportService
    {
        Task<List<ResponseSportDTO>> GetAll(CancellationToken cancellationToken);
        Task<List<ResponseSportDTO>> GetByIds(List<Guid> ids, CancellationToken cancellationToken);
        Task<ResponseIdDTO> Create(Guid userId, CreateSportDTO dto, CancellationToken cancellationToken);
        Task Delete(Guid userId, Guid id, CancellationToken cancellationToken);
    }
}