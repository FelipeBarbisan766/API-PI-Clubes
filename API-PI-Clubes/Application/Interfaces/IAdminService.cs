using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces
{
    public interface IAdminService
    {
        Task<ResponseAdminDTO> GetById(Guid id);
        Task Create(CreatAdminDTO dto);
        Task<ResponseAdminDTO> Update(Guid id, UpdateAdminDTO dto);
        Task Delete(Guid id);
    }
}
