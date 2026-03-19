using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces
{
    public interface IReserveService
    {
        Task<IEnumerable<ResponseReserveDTO>> GetAll();
        Task<ResponseReserveDTO> GetById(Guid id);
        Task<ResponseReserveDTO> Create(CreatReserveDTO dto);
        Task<ResponseReserveDTO> Update(Guid id, UpdateReserveDTO dto);
        Task Delete(Guid id);
    }
}
