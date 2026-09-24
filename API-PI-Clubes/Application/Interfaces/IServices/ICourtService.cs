using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface ICourtService
    {
        Task<PagedResultDTO<ResponseCourtDTO>> GetAll(CourtQueryDTO query, CancellationToken cancellationToken);
        Task<ResponseCourtDTO> GetById(Guid id, CancellationToken cancellationToken);
        Task<List<ResponseCourtDTO>> GetByClubId(Guid id, CancellationToken cancellationToken);
        Task<ResponseIdDTO> Create(Guid userId, CreatCourtDTO dto, CancellationToken cancellationToken);
        Task<ResponseCourtDTO> Update(Guid userId, Guid id, UpdateCourtDTO dto, CancellationToken cancellationToken);
        Task Delete(Guid userId, Guid id, CancellationToken cancellationToken);
        Task AddMoreImagesAsync(Guid userId, Guid id, UploadImageDTO dto, CancellationToken cancellationToken);
        Task DeleteImageAsync(Guid userId, Guid id, Guid imageId, CancellationToken cancellationToken);
        Task ReorderImagesAsync(Guid userId, Guid id, List<ReorderImageDTO> orders, CancellationToken cancellationToken);
    }
}
