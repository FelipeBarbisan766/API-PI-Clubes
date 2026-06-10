using System.Security.Claims;
using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface IPlayerService
    {
        Task<ResponsePlayerDTO> GetById(Guid id);
        Task<ResponsePlayerDTO> GetCurrentUserInfo(ClaimsPrincipal user);
        Task<ResponseIdDTO> Create(Guid id);
        Task<ResponsePlayerDTO> Update(Guid id, UpdatePlayerDTO dto);
        Task Delete(Guid id);
    }
}
