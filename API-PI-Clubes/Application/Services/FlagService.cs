using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Exceptions;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Services;

public class FlagService : IFlagService
{
    private readonly IFlagRepository _repository;
    private readonly IPlayerRepository _playerRepository;
    private readonly IUserRepository _userRepository;

    public FlagService(IFlagRepository flagRepository, IPlayerRepository playerRepository, IUserRepository userRepository)
    {
        _repository = flagRepository;
        _playerRepository = playerRepository;
        _userRepository = userRepository;
    }

    public async Task<List<FlagDTO.FlagDto>> GetAll(CancellationToken cancellationToken)
    {
        var flags = await _repository.GetAllAsync(cancellationToken);
        return flags.Select(f => new FlagDTO.FlagDto(f.Id, f.Name, f.Description)).ToList();
    }
    public async Task<FlagDTO.FlagDto> Create(Guid userId, FlagDTO.CreateFlagDTO dto, CancellationToken cancellationToken)
    {
        await AuthorizeRoot(userId, cancellationToken);
        var flag = new Flag
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    
        await _repository.AddAsync(flag, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return new FlagDTO.FlagDto(flag.Id, flag.Name, flag.Description);
    }
    public async Task Delete(Guid userId, Guid id, CancellationToken cancellationToken)
    {
        ValidateId(id);
        await AuthorizeRoot(userId, cancellationToken);

        if (!await _repository.ExistsAsync(id, cancellationToken))
            throw new NotFoundException("Flag", id);

        await _repository.DeleteAsync(id, cancellationToken);
    }
    
    public async Task<FlagDTO.PlayerFlagDto> FlagPlayer(
        Guid playerUserId, Guid adminUserId, FlagDTO.CreatePlayerFlagRequest request, CancellationToken cancellationToken)
    {
        var player = await _playerRepository.GetByUserIdAsync(playerUserId, cancellationToken)
            ?? throw new NotFoundException("Player não encontrado");

        var flag = await _repository.GetByIdAsync(request.FlagId, cancellationToken)
            ?? throw new NotFoundException("Tipo de flag não encontrado");

        if (request.ReserveId.HasValue &&
            await _repository.ExistsForReserveAsync(request.ReserveId.Value, request.FlagId, cancellationToken))
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

        await _repository.AddPlayerFlagAsync(flagPlayer, cancellationToken);

        return new FlagDTO.PlayerFlagDto(
            flagPlayer.Id, flag.Id, flag.Name, flagPlayer.ReserveId, flagPlayer.Notes, flagPlayer.CreatedAt);
    }

    public async Task<List<FlagDTO.PlayerFlagDto>> GetByPlayerAsync(Guid playerUserId, CancellationToken cancellationToken)
    {
        var player = await _playerRepository.GetByUserIdAsync(playerUserId, cancellationToken)
            ?? throw new NotFoundException("Player não encontrado");

        var flags = await _repository.GetByPlayerIdAsync(player.Id, cancellationToken);

        return flags
            .Select(fp => new FlagDTO.PlayerFlagDto(fp.Id, fp.FlagId, fp.Flag.Name, fp.ReserveId, fp.Notes, fp.CreatedAt))
            .ToList();
    }
    
    private static void ValidateId(Guid id)
    {
        if (id == Guid.Empty)
            throw new ValidationException("O ID informado é inválido.");
    }

    private async Task AuthorizeRoot(Guid userId, CancellationToken cancellationToken)
    {
        var isRoot = await _userRepository.IsRootAsync(userId, cancellationToken);
        if (!isRoot)
            throw new ForbiddenException("Você não tem permissão para gerenciar.");

    }
}