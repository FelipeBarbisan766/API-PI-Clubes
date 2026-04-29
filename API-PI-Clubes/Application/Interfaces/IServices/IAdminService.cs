using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface IAdminService
    {
        Task<ResponseAdminDTO> GetById(Guid id);
        Task<ResponseAdminDTO> GetByUserId(Guid id);
        Task<ResponseIdDTO> Create(CreatAdminDTO dto);
        Task<ResponseAdminDTO> Update(Guid id, UpdateAdminDTO dto);
        Task Delete(Guid id);
    }
}
