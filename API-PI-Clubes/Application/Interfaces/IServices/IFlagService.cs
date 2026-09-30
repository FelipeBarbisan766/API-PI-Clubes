using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices;

public interface IFlagService
{
    Task<List<FlagDTO.FlagDto>> GetAll(CancellationToken cancellationToken);
    Task<FlagDTO.PlayerFlagDto> Create(Guid playerUserId, Guid adminUserId, FlagDTO.CreatePlayerFlagRequest request, CancellationToken cancellationToken);
    Task<List<FlagDTO.PlayerFlagDto>> GetByPlayerAsync(Guid playerUserId, CancellationToken cancellationToken);
}