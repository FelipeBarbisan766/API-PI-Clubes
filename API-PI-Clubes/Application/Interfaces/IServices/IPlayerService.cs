using System.Security.Claims;
using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface IPlayerService
    {
        Task<ResponsePlayerDTO> GetById(Guid id, CancellationToken cancellationToken);
        Task<ResponsePlayerDTO> GetCurrentUserInfo(Guid id, CancellationToken cancellationToken);
        Task<ResponseIdDTO> Create(Guid id, CancellationToken cancellationToken);
        Task<ResponsePlayerDTO> Update(Guid userId, Guid id, UpdatePlayerDTO dto, CancellationToken cancellationToken);
        Task Delete(Guid userId, CancellationToken cancellationToken);
        Task<ResponsePlayerDTO> GetByProfileName(string profileName, CancellationToken cancellationToken);
        Task<ResponsePlayerDTO> SetProfileName(Guid userId, SetProfileNameDTO dto, CancellationToken cancellationToken);
        Task<List<ResponseSportDTO>> GetFavoriteSports(Guid userId, CancellationToken cancellationToken);
        Task<List<ResponseSportDTO>> AddFavoriteSports(Guid userId, AddFavoriteSportsDTO dto, CancellationToken cancellationToken);
        Task<List<ResponseSportDTO>> SetFavoriteSports(Guid userId, SetFavoriteSportsDTO dto, CancellationToken cancellationToken);
    }
}
