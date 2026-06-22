using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Infrastructure.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly AppDbContext _context;

        public ScheduleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Schedule>> GetAllAsync()
        {
            return await _context.Schedules
                .Where(c => c.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Schedule>> GetByCourtIdAsync(Guid courtId)
        {
            return await _context.Schedules
                .Where(c => c.CourtId == courtId && c.IsActive)
                .ToListAsync();
        }

        public async Task<Schedule?> GetByIdAsync(Guid id)
        {
            return await _context.Schedules
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
        }
        public async Task<IEnumerable<Schedule>> GetByCourtAndDateAsync(Guid courtId, DateOnly date)
        {
            // Intervalo da data para filtrar as reservas pelo dia exato
            var dateStart = date.ToDateTime(TimeOnly.MinValue);
            var dateEnd   = date.AddDays(1).ToDateTime(TimeOnly.MinValue);
 
            return await _context.Schedules
                .Where(s => s.CourtId == courtId
                            && s.DayOfWeek == dateStart.DayOfWeek
                            && s.IsActive)
                .Include(s => s.Reserves
                    .Where(r => r.Date >= dateStart && r.Date < dateEnd && r.IsActive))
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Schedules
                .AnyAsync(s => s.Id == id && s.IsActive);
        }

        public async Task AddAsync(Schedule schedule)
        {
            await _context.Schedules.AddAsync(schedule);
        }

        public void Update(Schedule schedule)
        {
            _context.Schedules.Update(schedule);
        }

        public async Task DeleteAsync(Guid id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule != null)
            {
                schedule.IsActive = false;
                schedule.UpdatedAt = DateTime.UtcNow;
                _context.Schedules.Update(schedule);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}