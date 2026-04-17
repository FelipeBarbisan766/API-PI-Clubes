using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface ICourtService
    {
        Task<IEnumerable<ResponseCourtDTO>> GetAll();
        Task<ResponseCourtDTO> GetById(Guid id);
        Task<ResponseIdDTO> Create(CreatCourtDTO dto);
        Task<ResponseCourtDTO> Update(Guid id, UpdateCourtDTO dto);
        Task Delete(Guid id);
    }
}
