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

        public async Task<IEnumerable<Schedule>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Schedules
                .Where(c => c.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Schedule>> GetByCourtIdAsync(Guid courtId, CancellationToken cancellationToken)
        {
            return await _context.Schedules
                .Where(c => c.CourtId == courtId && c.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<Schedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Schedules
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive, cancellationToken);
        }

        public async Task<IEnumerable<Schedule>> GetByCourtAndDateAsync(
            Guid courtId, DateOnly date, DateTime earliestBookable, CancellationToken cancellationToken)
        {
            var earliestDate = DateOnly.FromDateTime(earliestBookable);

            if (date < earliestDate)
                return Enumerable.Empty<Schedule>();

            var dateStart = date.ToDateTime(TimeOnly.MinValue);
            var dateEnd = date.AddDays(1).ToDateTime(TimeOnly.MinValue);

            var query = _context.Schedules
                .Where(s => s.CourtId == courtId
                            && s.DayOfWeek == dateStart.DayOfWeek
                            && s.IsActive);

            if (date == earliestDate)
            {
                var minTime = TimeOnly.FromDateTime(earliestBookable);
                query = query.Where(s => s.StartTime >= minTime);
            }

            return await query
                .Include(s => s.Reserves
                    .Where(r => r.Date >= dateStart && r.Date < dateEnd && r.IsActive))
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Schedules
                .AnyAsync(s => s.Id == id && s.IsActive, cancellationToken);
        }

        public async Task<IEnumerable<Schedule>> GetByCourtAndDaysOfWeekAsync(Guid courtId, List<DayOfWeek> daysOfWeek,
            CancellationToken cancellationToken)
        {
            return await _context.Schedules
                .Where(s => s.CourtId == courtId
                            && daysOfWeek.Contains(s.DayOfWeek)
                            && s.IsActive)
                .ToListAsync(cancellationToken);
        }


        public async Task AddRangeAsync(IEnumerable<Schedule> schedules, CancellationToken cancellationToken)
        {
            await _context.Schedules.AddRangeAsync(schedules, cancellationToken);
        }

        public async Task AddAsync(Schedule schedule, CancellationToken cancellationToken)
        {
            await _context.Schedules.AddAsync(schedule, cancellationToken);
        }

        public void Update(Schedule schedule)
        {
            _context.Schedules.Update(schedule);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var schedule = await _context.Schedules.FindAsync(new object[] { id }, cancellationToken);
            if (schedule != null)
            {
                schedule.IsActive = false;
                schedule.UpdatedAt = DateTime.UtcNow;
                _context.Schedules.Update(schedule);
            }
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> IsOwnedByUserAsync(Guid Id, Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Schedules
                .AnyAsync(c => c.Id == Id && c.Court.Club.ClubAdmin.Any(a => a.Admin.UserId == userId),
                    cancellationToken);
        }
    }
}