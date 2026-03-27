using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces
{
    public interface IClubService
    {
        Task<IEnumerable<ResponseClubDTO>> GetAll();
        Task<ResponseClubByIdDTO> GetById(Guid id);
        Task Create(CreateClubDTO dto);
        Task<ResponseClubDTO> Update(Guid id, UpdateClubDTO dto);
        Task Delete(Guid id);
    }
}
