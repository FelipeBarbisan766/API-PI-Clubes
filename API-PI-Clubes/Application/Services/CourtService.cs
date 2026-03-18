using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using API_PI_Clubes.Infrastructure.Data;

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


    }
}
