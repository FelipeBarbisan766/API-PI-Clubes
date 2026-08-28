using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface ICourtService
    {
        Task<PagedResultDTO<ResponseCourtDTO>> GetAll(CourtQueryDTO query);
        Task<ResponseCourtDTO> GetById(Guid id);
        Task<List<ResponseCourtDTO>> GetByClubId(Guid id);
        Task<ResponseIdDTO> Create(CreatCourtDTO dto);
        Task<ResponseCourtDTO> Update(Guid userId, Guid id, UpdateCourtDTO dto);
        Task Delete(Guid userId, Guid id);
        Task AddMoreImagesAsync(Guid userId, Guid id, UploadImageDTO dto);
        Task DeleteImageAsync(Guid userId, Guid id, Guid imageId);
        Task ReorderImagesAsync(Guid userId, Guid id, List<ReorderImageDTO> orders);
    }
}
