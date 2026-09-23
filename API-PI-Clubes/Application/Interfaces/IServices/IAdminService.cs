using System.Security.Claims;
using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface IAdminService
    {
        Task<ResponseAdminDTO> GetById(Guid id, CancellationToken cancellationToken);
        Task<ResponseAdminDTO> GetCurrentUserInfo(Guid id, CancellationToken cancellationToken);
        
        Task<ResponseIdDTO> Create(Guid id, CancellationToken cancellationToken);
        Task<ResponseAdminDTO> Update(Guid userId, Guid id, UpdateAdminDTO dto, CancellationToken cancellationToken);
        Task Delete(Guid userId, Guid id, CancellationToken cancellationToken);
    }
}
