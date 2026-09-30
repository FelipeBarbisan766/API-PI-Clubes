using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Exceptions;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Services;

public class FlagService : IFlagService
{
    private readonly IFlagRepository _flagRepository;
    private readonly IPlayerRepository _playerRepository;

    public FlagService(IFlagRepository flagRepository, IPlayerRepository playerRepository)
    {
        _flagRepository = flagRepository;
        _playerRepository = playerRepository;
    }

    public async Task<List<FlagDTO.FlagDto>> GetAll(CancellationToken cancellationToken)
    {
        var flags = await _flagRepository.GetAllAsync(cancellationToken);
        return flags.Select(f => new FlagDTO.FlagDto(f.Id, f.Name, f.Description)).ToList();
    }

    public async Task<FlagDTO.PlayerFlagDto> Create(
        Guid playerUserId, Guid adminUserId, FlagDTO.CreatePlayerFlagRequest request, CancellationToken cancellationToken)
    {
        var player = await _playerRepository.GetByUserIdAsync(playerUserId, cancellationToken)
            ?? throw new NotFoundException("Player não encontrado");

        var flag = await _flagRepository.GetByIdAsync(request.FlagId, cancellationToken)
            ?? throw new NotFoundException("Tipo de flag não encontrado");

        if (request.ReserveId.HasValue &&
            await _flagRepository.ExistsForReserveAsync(request.ReserveId.Value, request.FlagId, cancellationToken))
        {
            throw new ConflictException("Essa flag já foi aplicada para essa reserva");
        }

        var flagPlayer = new FlagPlayer
        {
            PlayerId = player.Id,
            FlagId = flag.Id,
            ReserveId = request.ReserveId,
            CreatedByAdminId = adminUserId,
            Notes = request.Notes
        };

        await _flagRepository.AddPlayerFlagAsync(flagPlayer, cancellationToken);

        return new FlagDTO.PlayerFlagDto(
            flagPlayer.Id, flag.Id, flag.Name, flagPlayer.ReserveId, flagPlayer.Notes, flagPlayer.CreatedAt);
    }

    public async Task<List<FlagDTO.PlayerFlagDto>> GetByPlayerAsync(Guid playerUserId, CancellationToken cancellationToken)
    {
        var player = await _playerRepository.GetByUserIdAsync(playerUserId, cancellationToken)
            ?? throw new NotFoundException("Player não encontrado");

        var flags = await _flagRepository.GetByPlayerIdAsync(player.Id, cancellationToken);

        return flags
            .Select(fp => new FlagDTO.PlayerFlagDto(fp.Id, fp.FlagId, fp.Flag.Name, fp.ReserveId, fp.Notes, fp.CreatedAt))
            .ToList();
    }
}