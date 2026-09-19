using System.Security.Claims;
using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Exceptions;
using API_PI_Clubes.Application.Interfaces.IMappers;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.Enums;
using Microsoft.Data.SqlClient;
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
        private readonly ISportService _sportService;

        public PlayerService(IPlayerMapper mapper, IPlayerRepository repository,
            IUserService userService, ISportRepository sportRepository, ISportService sportService)
        {
            _mapper = mapper;
            _repository = repository;
            _userService = userService;
            _sportRepository = sportRepository;
            _sportService = sportService;
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
            if (sportIds == null) return; 

            var distinctIds = sportIds.Distinct().ToList();
            if (distinctIds.Count == 0) return;

            var existingCount = await _sportRepository.CountExistingAsync(distinctIds);
            if (existingCount != distinctIds.Count)
                throw new ValidationException("Um ou mais esportes informados são inválidos.");
        }

        public async Task Delete(Guid userId)
        {
            var id = await GetPlayerId(userId);
            await AuthorizeOwnership(userId, id);

            var exists = await _repository.ExistsAsync(id);

            if (!exists)
                throw new NotFoundException("Jogador", id);

            await _repository.DeleteAsync(id);
        }

        public async Task<ResponsePlayerDTO> GetByProfileName(string profileName)
        {
            if (string.IsNullOrWhiteSpace(profileName))
                throw new ValidationException("O nome de perfil informado é inválido.");

            var data = await _repository.GetByProfileNameWithFavoriteSportsAsync(profileName);

            if (data == null)
                throw new NotFoundException("Jogador", profileName);

            return _mapper.ToDTO(data);
        }

        public async Task<ResponsePlayerDTO> SetProfileName(Guid userId, SetProfileNameDTO dto)
        {
            var id = await GetPlayerId(userId);
            await AuthorizeOwnership(userId, id);

            var alreadyTaken = await _repository.ExistsByProfileNameAsync(dto.ProfileName, id);
            if (alreadyTaken)
                throw new ConflictException("Esse nome de perfil já está em uso.");

            var data = await _repository.GetByIdWithFavoriteSportsAsync(id);
            if (data == null)
                throw new NotFoundException("Jogador", id);

            data.ProfileName = dto.ProfileName;
            data.UpdatedAt = DateTime.UtcNow;

            _repository.Update(data);

            try
            {
                await _repository.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                throw new ConflictException("Esse nome de perfil já está em uso.");
            }

            return _mapper.ToDTO(data);
        }
        public async Task<List<ResponseSportDTO>> GetFavoriteSports(Guid userId)
        {
            var id = await GetPlayerId(userId);

            var data = await _repository.GetByIdWithFavoriteSportsAsync(id);
            if (data == null)
                throw new NotFoundException("Jogador", id);

            var sportIds = data.FavoriteSports.Select(fs => fs.SportId).ToList();
            return await _sportService.GetByIds(sportIds);
        }

        public async Task<List<ResponseSportDTO>> AddFavoriteSports(Guid userId, AddFavoriteSportsDTO dto)
        {
            var id = await GetPlayerId(userId);
            ValidateAddFavoriteSportsDTO(dto);
            await ValidateSportIdsAsync(dto.SportIds);
            await AuthorizeOwnership(userId, id);

            var data = await _repository.GetByIdWithFavoriteSportsAsync(id);
            if (data == null)
                throw new NotFoundException("Jogador", id);

            var currentIds = data.FavoriteSports.Select(fs => fs.SportId).ToHashSet();
            var toAdd = dto.SportIds.Distinct().Where(sid => !currentIds.Contains(sid));

            foreach (var sportId in toAdd)
                data.FavoriteSports.Add(new PlayerFavoriteSport { PlayerId = data.Id, SportId = sportId });

            _repository.Update(data);
            await _repository.SaveChangesAsync();

            var sportIds = data.FavoriteSports.Select(fs => fs.SportId).ToList();
            return await _sportService.GetByIds(sportIds);

            static void ValidateAddFavoriteSportsDTO(AddFavoriteSportsDTO dto)
            {
                if (dto?.SportIds == null || dto.SportIds.Count == 0)
                    throw new ValidationException("Informe ao menos um esporte.");
            }
        }
        public async Task<List<ResponseSportDTO>> SetFavoriteSports(Guid userId, SetFavoriteSportsDTO dto)
        {
            var id = await GetPlayerId(userId);
            ValidateSetFavoriteSportsDTO(dto);
            await ValidateSportIdsAsync(dto.SportIds);
            await AuthorizeOwnership(userId, id);

            var data = await _repository.GetByIdWithFavoriteSportsAsync(id);
            if (data == null)
                throw new NotFoundException("Jogador", id);

            var newIds = dto.SportIds.Distinct().ToHashSet();
            var currentIds = data.FavoriteSports.Select(fs => fs.SportId).ToHashSet();

            var toAdd = newIds.Where(sid => !currentIds.Contains(sid));
            var toRemove = data.FavoriteSports.Where(fs => !newIds.Contains(fs.SportId)).ToList();

            foreach (var sportId in toAdd)
                data.FavoriteSports.Add(new PlayerFavoriteSport { PlayerId = data.Id, SportId = sportId });

            foreach (var favoriteSport in toRemove)
                data.FavoriteSports.Remove(favoriteSport);

            _repository.Update(data);
            await _repository.SaveChangesAsync();

            var sportIds = data.FavoriteSports.Select(fs => fs.SportId).ToList();
            return await _sportService.GetByIds(sportIds);

            static void ValidateSetFavoriteSportsDTO(SetFavoriteSportsDTO dto)
            {
                if (dto?.SportIds == null)
                    throw new ValidationException("Informe a lista de esportes favoritos.");
            }
        }
        // --------------------------------------
        private static void ValidateAddFavoriteSportsDTO(AddFavoriteSportsDTO dto)
        {
            if (dto?.SportIds == null || dto.SportIds.Count == 0)
                throw new ValidationException("Informe ao menos um esporte.");
        }        

        private static bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            return ex.InnerException is SqlException sqlEx &&
                   (sqlEx.Number == 2601 || sqlEx.Number == 2627);
        }

        private async Task<Guid> GetPlayerId(Guid userId)
        {
            var playerId = await _repository.GetIdByUserIdAsync(userId);
            if (playerId == null)
                throw new NotFoundException("Não foi possivel buscar seu PlayerId com base no seu Id", userId);
            return playerId;
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