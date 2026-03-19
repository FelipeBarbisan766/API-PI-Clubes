using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<ResponseUserDTO>> GetAll();
        Task<ResponseUserDTO> GetById(Guid id);
        Task<ResponseUserDTO> Create(CreatUserDTO dto);
        Task<ResponseUserDTO> Update(Guid id, UpdateUserDTO dto);
        Task Delete(Guid id);
    }
}
