using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface IShareService
    {
        Task<ShareImageResultDTO?> GetClubImageAsync(Guid clubId, CancellationToken cancellationToken);
    }
}