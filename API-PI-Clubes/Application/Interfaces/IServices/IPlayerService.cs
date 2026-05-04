using System.Security.Claims;
using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface IPlayerService
    {
        Task<IEnumerable<ResponsePlayerDTO>> GetAll();
        Task<ResponsePlayerDTO> GetById(Guid id);
        Task<ResponsePlayerDTO> GetCurrentUserInfo(ClaimsPrincipal user);
        Task<ResponseIdDTO> Create(CreatPlayerDTO dto);
        Task<ResponsePlayerDTO> Update(Guid id, UpdatePlayerDTO dto);
        Task Delete(Guid id);
    }
}
