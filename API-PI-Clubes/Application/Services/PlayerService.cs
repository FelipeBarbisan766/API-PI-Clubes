using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.Enums;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Application.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        public PlayerService(AppDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<IEnumerable<ResponsePlayerDTO>> GetAll()
        {
            return await _context.Players
                .Where(c => c.IsActive)
                .Select(c => new ResponsePlayerDTO
                {
                    Id = c.Id,
                    UserName = c.UserName,
                    ContactNumber = c.ContactNumber,
                    Description = c.Description,
                    RankCategory = c.RankCategory,
                    UserId = c.UserId
                })
                .ToListAsync();
        }
        public async Task<ResponsePlayerDTO> GetById(Guid id)
        {
            var data = await _context.Players
                .Where(c => c.Id == id && c.IsActive)
                .Select(c => new ResponsePlayerDTO
                {
                    Id = c.Id,
                    UserName = c.UserName,
                    ContactNumber = c.ContactNumber,
                    Description = c.Description,
                    RankCategory = c.RankCategory,
                    UserId = c.UserId
                })
                .FirstOrDefaultAsync();

            if (data == null)
                throw new Exception("Player not found");

            return data;
        }

        public async Task<ResponseIdDTO> Create(CreatPlayerDTO dto)
        {
            var entity = new Player
            {
                UserName = dto.UserName,
                ContactNumber = dto.ContactNumber,
                Description = dto.Description,
                UserId = dto.UserId,
                RankCategory = RankCategoryEnum.none
            };
            using var transaction = await _context.Database.BeginTransactionAsync();
            await _userService.UpdateRole(dto.UserId, RoleEnum.Player);

            _context.Players.Add(entity);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ResponseIdDTO
            {
                Id = entity.Id
            };

        }

        public async Task<ResponsePlayerDTO> Update(Guid id, UpdatePlayerDTO dto)
        {
            var data = await _context.Players.FirstOrDefaultAsync(c => c.Id == id);

            if (data == null)
                throw new Exception("Player not found");

            data.UserName = dto.UserName;
            data.ContactNumber = dto.ContactNumber;
            data.Description = dto.Description;
            data.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ResponsePlayerDTO
            {
                Id = data.Id,
                UserName = data.UserName,
                ContactNumber = data.ContactNumber,
                Description = data.Description,
                RankCategory = data.RankCategory,
                UserId = data.UserId
            };
        }

        public async Task Delete(Guid id)
        {
            var data = await _context.Players.FirstOrDefaultAsync(c => c.Id == id);

            if (data == null)
                throw new Exception("Player not found");

            data.IsActive = false;
            data.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
