using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface IClubService
    {
        Task<PagedResultDTO<ResponseClubDTO>> GetAll(ClubQueryDTO query, CancellationToken cancellationToken);
        Task<ResponseClubByIdDTO> GetById(Guid id, CancellationToken cancellationToken);
        Task<List<ResponseClubDTO>> GetAllByAdminId(Guid id, CancellationToken cancellationToken);
        Task<ResponseDashboardDTO> GetDashboard(Guid id, CancellationToken cancellationToken);
        Task<ResponseIdDTO> Create(Guid userId,CreateClubDTO dto, CancellationToken cancellationToken);
        Task<ResponseClubDTO> Update(Guid userId, Guid id, UpdateClubDTO dto, CancellationToken cancellationToken);
        Task Delete(Guid userId,Guid id, CancellationToken cancellationToken);
        Task AddMoreImagesAsync(Guid userId, Guid id, UploadImageDTO dto, CancellationToken cancellationToken);
        Task DeleteImageAsync(Guid userId, Guid id, Guid imageId, CancellationToken cancellationToken);
        Task ReorderImagesAsync(Guid userId, Guid id, List<ReorderImageDTO> orders, CancellationToken cancellationToken);
    }
}
