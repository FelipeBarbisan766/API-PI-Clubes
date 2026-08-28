using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface IClubService
    {
        Task<PagedResultDTO<ResponseClubDTO>> GetAll(ClubQueryDTO query);
        Task<ResponseClubByIdDTO> GetById(Guid id);
        Task<List<ResponseClubDTO>> GetAllByAdminId(Guid id);
        Task<ResponseDashboardDTO> GetDashboard(Guid id);
        Task<ResponseIdDTO> Create(CreateClubDTO dto);
        Task<ResponseClubDTO> Update(Guid userId, Guid id, UpdateClubDTO dto);
        Task Delete(Guid userId,Guid id);
        Task AddMoreImagesAsync(Guid userId, Guid id, UploadImageDTO dto);
        Task DeleteImageAsync(Guid userId, Guid id, Guid imageId);
        Task ReorderImagesAsync(Guid userId, Guid id, List<ReorderImageDTO> orders);
    }
}
