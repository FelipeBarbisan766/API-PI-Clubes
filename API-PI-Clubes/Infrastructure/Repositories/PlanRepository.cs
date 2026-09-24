using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Infrastructure.Repositories
{
    public class PlanRepository : IPlanRepository
    {
        private readonly AppDbContext _context;

        public PlanRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Plan?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            => await _context.Plans.FindAsync(id,cancellationToken);
 
        public async Task<IEnumerable<Plan>> GetAllActiveAsync(CancellationToken  cancellationToken)
            => await _context.Plans
                .Where(p => p.IsActive)
                .OrderBy(p => p.Price)
                .ToListAsync(cancellationToken);
 
        public async Task AddAsync(Plan plan,CancellationToken  cancellationToken)
        {
            await _context.Plans.AddAsync(plan,cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
 
        public async Task UpdateAsync(Plan plan,CancellationToken  cancellationToken)
        {
            _context.Plans.Update(plan);
            await _context.SaveChangesAsync(cancellationToken);
        }

    
    }
}
