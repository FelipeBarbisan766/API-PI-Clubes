using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface ISportService
    {
        Task<List<ResponseSportDTO>> GetAll(CancellationToken cancellationToken);
        Task<List<ResponseSportDTO>> GetByIds(List<Guid> ids, CancellationToken cancellationToken);
        Task<List<ResponseSportBackofficeDTO>> GetAllForBackoffice(bool includeInactive, CancellationToken cancellationToken);
        Task<ResponseSportBackofficeDTO> Create(SaveSportDTO dto, CancellationToken cancellationToken);
        Task<ResponseSportBackofficeDTO> Update(Guid id, SaveSportDTO dto, CancellationToken cancellationToken);
        Task SetActive(Guid id, bool isActive, CancellationToken cancellationToken);
    }
}