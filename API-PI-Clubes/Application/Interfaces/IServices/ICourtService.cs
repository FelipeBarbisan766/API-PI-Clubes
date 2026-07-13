using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface ICourtService
    {
        Task<PagedResultDTO<ResponseCourtDTO>> GetAll(CourtQueryDTO query);
        Task<ResponseCourtDTO> GetById(Guid id);
        Task<List<ResponseCourtDTO>> GetByClubId(Guid id);
        Task<ResponseIdDTO> Create(CreatCourtDTO dto);
        Task<ResponseCourtDTO> Update(Guid id, UpdateCourtDTO dto);
        Task AddMoreImagesAsync(Guid id, UploadImageDTO dto);
        Task Delete(Guid id);
    }
}
