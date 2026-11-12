using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Infrastructure.Repositories
{
    public class RootRepository : IRootRepository
    {
        private readonly AppDbContext _context;

        public RootRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OverviewRootDTO> GetOverviewAsync(CancellationToken cancellationToken)
        {
            return new OverviewRootDTO
            {
                TotalPlans = await _context.Plans.AsNoTracking().CountAsync(cancellationToken),
                TotalClubs = await _context.Clubs.AsNoTracking().CountAsync(cancellationToken),
                TotalSports = await _context.Sports.AsNoTracking().CountAsync(cancellationToken),
                TotalUsers = await _context.Users.AsNoTracking().CountAsync(cancellationToken),
                TotalFlags = await _context.Flags.AsNoTracking().CountAsync(cancellationToken)
            };
        }
    }
}