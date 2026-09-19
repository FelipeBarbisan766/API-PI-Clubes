using System.Security.Claims;
using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface IPlayerService
    {
        Task<ResponsePlayerDTO> GetById(Guid id);
        Task<ResponsePlayerDTO> GetCurrentUserInfo(Guid id);
        Task<ResponseIdDTO> Create(Guid id);
        Task<ResponsePlayerDTO> Update(Guid userId, Guid id, UpdatePlayerDTO dto);
        Task Delete(Guid userId);
        Task<ResponsePlayerDTO> GetByProfileName(string profileName);
        Task<ResponsePlayerDTO> SetProfileName(Guid userId, SetProfileNameDTO dto);
        Task<List<ResponseSportDTO>> GetFavoriteSports(Guid userId);
        Task<List<ResponseSportDTO>> AddFavoriteSports(Guid userId, AddFavoriteSportsDTO dto);
        Task<List<ResponseSportDTO>> SetFavoriteSports(Guid userId, SetFavoriteSportsDTO dto);
    }
}
