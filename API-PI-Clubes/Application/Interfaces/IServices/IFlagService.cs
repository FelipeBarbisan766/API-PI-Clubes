using API_PI_Clubes.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API_PI_Clubes.Application.Interfaces.IServices;

public interface IFlagService
{
    Task<List<FlagDTO.FlagDto>> GetAll(CancellationToken cancellationToken);
    Task<FlagDTO.FlagDto> Create(Guid userId,FlagDTO.CreateFlagDTO dto, CancellationToken cancellationToken);
    Task Delete(Guid userId, Guid id, CancellationToken cancellationToken);
    Task<FlagDTO.PlayerFlagDto> FlagPlayer(Guid playerUserId, Guid adminUserId, FlagDTO.CreatePlayerFlagRequest request, CancellationToken cancellationToken);
    Task<List<FlagDTO.PlayerFlagDto>> GetByPlayerAsync(Guid playerUserId, CancellationToken cancellationToken);
}