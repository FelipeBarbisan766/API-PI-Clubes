using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.Enums;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Application.Services
{
    public class ReserveService : IReserveService
    {
        private readonly AppDbContext _context;
        public ReserveService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ResponseReserveDTO>> GetAll()
        {
            return await _context.Reserves
                .Where(c => c.IsActive)
                .Select(c => new ResponseReserveDTO
                {
                    Id = c.Id,
                    Date = c.Date,
                    Status = c.Status
                })
                .ToListAsync();
        }
        public async Task<ResponseReserveDTO> GetById(Guid id)
        {
            var data = await _context.Reserves
                .Where(c => c.Id == id && c.IsActive)
                .Select(c => new ResponseReserveDTO
                {
                    Id = c.Id,
                    Date = c.Date,
                    Status = c.Status
                })
                .FirstOrDefaultAsync();

            if (data == null)
                throw new Exception("Reserve not found");

            return data;
        }

        public async Task<ResponseIdDTO> Create(CreatReserveDTO dto)
        {
            var entity = new Reserve
            {
                Date = dto.Date,
                Status = StatusEnum.AguardandoConfirmacao,
                ScheduleId = dto.ScheduleId,
                PlayerId = dto.PlayerId
            };

            _context.Reserves.Add(entity);
            await _context.SaveChangesAsync();

            return new ResponseIdDTO
            {
                Id = entity.Id
            };
        }

        public async Task<ResponseReserveDTO> Update(Guid id, UpdateReserveDTO dto)
        {
            var data = await _context.Reserves.FirstOrDefaultAsync(c => c.Id == id);

            if (data == null)
                throw new Exception("Reserve not found");

            data.Date = dto.Date;
            data.Status = dto.Status;
            data.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ResponseReserveDTO
            {
                Id = data.Id,
                Date = data.Date,
                Status = data.Status
            };
        }

        public async Task Delete(Guid id)
        {
            var data = await _context.Reserves.FirstOrDefaultAsync(c => c.Id == id);

            if (data == null)
                throw new Exception("Reserve not found");

            data.IsActive = false;
            data.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
