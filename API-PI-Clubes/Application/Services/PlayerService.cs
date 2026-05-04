using System.Security.Claims;
using API_PI_Clubes.Application.DTOs;
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

        public PlayerService(IPlayerMapper mapper, IPlayerRepository repository, IUserService userService)
        {
            _mapper = mapper;
            _repository = repository;
            _userService = userService;
        }

        public async Task<IEnumerable<ResponsePlayerDTO>> GetAll()
        {
            var data = await _repository.GetAllAsync();
            return _mapper.ToDTO(data);
        }

        public async Task<ResponsePlayerDTO> GetById(Guid id)
        {
            ValidateId(id);

            var data = await _repository.GetByIdAsync(id);

            if (data == null)
                throw new InvalidOperationException("Player not found");

            return _mapper.ToDTO(data);
        }
        
        public async Task<ResponsePlayerDTO> GetCurrentUserInfo(ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new Exception("User ID not found in token");
            var entity = await _repository.GetByUserIdAsync(Guid.Parse(userId));
            if (entity == null)
                throw new Exception("User not found");

            return _mapper.ToDTO(entity);
        }
        
        public async Task<ResponseIdDTO> Create(CreatPlayerDTO dto)
        {
            ValidatePlayerDTO(dto);

            var strategy = _repository.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _repository.BeginTransactionAsync();

                try
                {
                    var user = await _userService.GetById(dto.UserId)
                               ?? throw new KeyNotFoundException("Usuário não encontrado");

                    var entity = new Player
                    {
                        UserName = dto.UserName,
                        ContactNumber = dto.ContactNumber,
                        Description = dto.Description,
                        RankCategory = RankCategoryEnum.none,
                        UserId = dto.UserId,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _repository.AddAsync(entity);

                    await _userService.UpdateRole(dto.UserId, RoleEnum.Player);

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

        public async Task<ResponsePlayerDTO> Update(Guid id, UpdatePlayerDTO dto)
        {
            ValidateId(id);
            ValidateUpdatePlayerDTO(dto);

            var data = await _repository.GetByIdAsync(id);

            if (data == null)
                throw new InvalidOperationException("Player not found");

            data.UserName = dto.UserName;
            data.ContactNumber = dto.ContactNumber;
            data.Description = dto.Description;
            data.RankCategory = RankCategoryEnum.none;
            data.UpdatedAt = DateTime.UtcNow;

            _repository.Update(data);
            await _repository.SaveChangesAsync();

            return _mapper.ToDTO(data);
        }

        public async Task Delete(Guid id)
        {
            ValidateId(id);

            var exists = await _repository.ExistsAsync(id);

            if (!exists)
                throw new InvalidOperationException("Player not found");

            await _repository.DeleteAsync(id);
        }

        private static void ValidateId(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid ID", nameof(id));
        }

        private static void ValidatePlayerDTO(CreatPlayerDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
        }

        private static void ValidateUpdatePlayerDTO(UpdatePlayerDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
        }
    }
}
