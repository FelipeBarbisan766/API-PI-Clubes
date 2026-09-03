using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Infrastructure.Repositories
{
    public class SportRepository : ISportRepository
    {
        private readonly AppDbContext _context;

        public SportRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> CountExistingAsync(List<Guid> ids)
        {
            return await _context.Sports
                .Where(s => s.IsActive && ids.Contains(s.Id))
                .CountAsync();
        }
    }
}