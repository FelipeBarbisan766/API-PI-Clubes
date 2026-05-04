using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface IClubService
    {
        Task<IEnumerable<ResponseClubDTO>> GetAll();
        Task<ResponseClubByIdDTO> GetById(Guid id);
        Task<List<ResponseClubDTO>> GetAllByAdminId(Guid id);
        Task<ResponseIdDTO> Create(CreateClubDTO dto);
        Task<ResponseClubDTO> Update(Guid id, UpdateClubDTO dto);
        Task Delete(Guid id);
        Task AddMoreImagesAsync(Guid id, UploadImageDTO dto);
    }
}
