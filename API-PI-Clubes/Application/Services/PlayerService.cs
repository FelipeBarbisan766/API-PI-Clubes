using System.Security.Claims;
using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Exceptions;
using API_PI_Clubes.Application.Interfaces.IMappers;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace API_PI_Clubes.Application.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _repository;
        private readonly IPlayerMapper _mapper;
        private readonly IUserService _userService;
        private readonly ISportRepository _sportRepository;

        public PlayerService(IPlayerMapper mapper, IPlayerRepository repository,
            IUserService userService, ISportRepository sportRepository)
        {
            _mapper = mapper;
            _repository = repository;
            _userService = userService;
            _sportRepository = sportRepository;
        }

        public async Task<ResponsePlayerDTO> GetById(Guid id)
        {
            ValidateId(id);

            var data = await _repository.GetByIdWithFavoriteSportsAsync(id);

            if (data == null)
                throw new NotFoundException("Jogador", id);

            return _mapper.ToDTO(data);
        }

        public async Task<ResponsePlayerDTO> GetCurrentUserInfo(Guid id)
        {
            var entity = await _repository.GetByUserIdWithFavoriteSportsAsync(id);
            if (entity == null)
                throw new NotFoundException("Usuário", id);
            return _mapper.ToDTO(entity);
        }
        
        public async Task<ResponseIdDTO> Create(Guid id)
        {
            var strategy = _repository.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _repository.BeginTransactionAsync();

                try
                {
                    var user = await _userService.GetById(id)
                               ?? throw new NotFoundException("Usuário", id);

                    var entity = new Player
                    {
                        RankCategory = RankCategoryEnum.none,
                        UserId = id,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _repository.AddAsync(entity);

                    //await _userService.UpdateRole(id, RoleEnum.Player);

                    await _repository.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new ResponseIdDTO { Id = entity.Id };
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }

            });
        }

        public async Task<ResponsePlayerDTO> Update(Guid userId, Guid id, UpdatePlayerDTO dto)
        {
            ValidateId(id);
            ValidateUpdatePlayerDTO(dto);
            await ValidateSportIdsAsync(dto.FavoriteSportIds);
            await AuthorizeOwnership(userId, id);

            var data = await _repository.GetByIdWithFavoriteSportsAsync(id);

            if (data == null)
                throw new NotFoundException("Jogador", id);

            data.RankCategory = RankCategoryEnum.none;
            data.UpdatedAt = DateTime.UtcNow;

            SyncFavoriteSports(data, dto.FavoriteSportIds);

            _repository.Update(data);
            await _repository.SaveChangesAsync();

            return _mapper.ToDTO(data);
        }

        private static void SyncFavoriteSports(Player player, List<Guid> newSportIds)
        {
            var newIds = newSportIds.Distinct().ToHashSet();
            var currentIds = player.FavoriteSports.Select(fs => fs.SportId).ToHashSet();

            var toRemove = player.FavoriteSports.Where(fs => !newIds.Contains(fs.SportId)).ToList();
            foreach (var fs in toRemove)
                player.FavoriteSports.Remove(fs);

            var toAdd = newIds.Where(sid => !currentIds.Contains(sid));
            foreach (var sportId in toAdd)
                player.FavoriteSports.Add(new PlayerFavoriteSport { PlayerId = player.Id, SportId = sportId });
        }

        private async Task ValidateSportIdsAsync(List<Guid> sportIds)
        {
            if (sportIds == null) return; // diferente do Court: favoritos podem ser esvaziados (lista vazia é válida)

            var distinctIds = sportIds.Distinct().ToList();
            if (distinctIds.Count == 0) return;

            var existingCount = await _sportRepository.CountExistingAsync(distinctIds);
            if (existingCount != distinctIds.Count)
                throw new ValidationException("Um ou mais esportes informados são inválidos.");
        }

        public async Task Delete(Guid userId, Guid id)
        {
            ValidateId(id);
            await AuthorizeOwnership(userId, id);

            var exists = await _repository.ExistsAsync(id);

            if (!exists)
                throw new NotFoundException("Jogador", id);

            await _repository.DeleteAsync(id);
        }
        private async Task AuthorizeOwnership(Guid userId, Guid id)
        {
            var isOwner = await _repository.IsOwnedByUserAsync(id, userId);
            if (!isOwner)
                throw new ForbiddenException("Você não tem permissão para gerenciar essa conta.");
        }

        private static void ValidateId(Guid id)
        {
            if (id == Guid.Empty)
                throw new ValidationException("O ID informado é inválido.");
        }

        private static void ValidateUpdatePlayerDTO(UpdatePlayerDTO dto)
        {
            if (dto == null)
                throw new ValidationException("Os dados de atualização são obrigatórios.");
        }
    }
}
