using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Application.Services
{
    public class CourtService : ICourtService
    {
        private readonly AppDbContext _context;
        public CourtService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ResponseCourtDTO>> GetAll()
        {
            return await _context.Courts
                .Where(c => c.IsActive)
                .Select(c => new ResponseCourtDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Type = c.Type,
                    Surface = c.Surface,
                    IsCovered = c.IsCovered,
                    PricePerHour = c.PricePerHour,
                    IsActive = c.IsActive,
                    Description = c.Description
                })
                .ToListAsync();
        }
        public async Task<ResponseCourtDTO> GetById(Guid id)
        {
            var data = await _context.Courts
                .Where(c => c.Id == id && c.IsActive)
                .Select(c => new ResponseCourtDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Type = c.Type,
                    Surface = c.Surface,
                    IsCovered = c.IsCovered,
                    PricePerHour = c.PricePerHour,
                    IsActive = c.IsActive,
                    Description = c.Description
                })
                .FirstOrDefaultAsync();

            if (data == null)
                throw new Exception("Court not found");

            return data;
        }

        public async Task<ResponseIdDTO> Create(CreatCourtDTO dto)
        {
            var entity = new Court
            {
                Name = dto.Name,
                Type = dto.Type,
                Surface = dto.Surface,
                IsCovered = dto.IsCovered,
                PricePerHour = dto.PricePerHour,
                Description = dto.Description,
                ClubId = dto.ClubId
            };

            _context.Courts.Add(entity);
            await _context.SaveChangesAsync();

            return new ResponseIdDTO
            {
                Id = entity.Id
            };
        }

        public async Task<ResponseCourtDTO> Update(Guid id, UpdateCourtDTO dto)
        {
            var data = await _context.Courts.FirstOrDefaultAsync(c => c.Id == id);

            if (data == null)
                throw new Exception("Court not found");

            data.Name = dto.Name;
            data.Type = dto.Type;
            data.Surface = dto.Surface;
            data.IsCovered = dto.IsCovered;
            data.PricePerHour = dto.PricePerHour;
            data.Description = dto.Description;
            data.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ResponseCourtDTO
            {
                Id = data.Id,
                Name = data.Name,
                Type = data.Type,
                Surface = data.Surface,
                IsCovered = data.IsCovered,
                PricePerHour = data.PricePerHour,
                IsActive = data.IsActive,
                Description = data.Description
            };
        }

        public async Task Delete(Guid id)
        {
            var data = await _context.Courts.FirstOrDefaultAsync(c => c.Id == id);

            if (data == null)
                throw new Exception("Court not found");

            data.IsActive = false;
            data.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }


    }
}
