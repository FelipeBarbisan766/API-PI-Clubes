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

        public async Task<Plan?> GetByIdAsync(Guid id)
            => await _context.Plans.FindAsync(id);
 
        public async Task<IEnumerable<Plan>> GetAllActiveAsync()
            => await _context.Plans
                .Where(p => p.IsActive)
                .OrderBy(p => p.Price)
                .ToListAsync();
 
        public async Task AddAsync(Plan plan)
        {
            await _context.Plans.AddAsync(plan);
            await _context.SaveChangesAsync();
        }
 
        public async Task UpdateAsync(Plan plan)
        {
            _context.Plans.Update(plan);
            await _context.SaveChangesAsync();
        }

    
    }
}
