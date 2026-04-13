using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface IClubService
    {
        Task<IEnumerable<ResponseClubDTO>> GetAll();
        Task<ResponseClubByIdDTO> GetById(Guid id);
        Task<ResponseIdDTO> Create(CreateClubDTO dto);
        Task<ResponseClubDTO> Update(Guid id, UpdateClubDTO dto);
        Task Delete(Guid id);
        Task Upload(Guid id, UploadImageDTO dto);
    Task DeleteImages(Guid id, DeleteImageDto dto);
    }
}
