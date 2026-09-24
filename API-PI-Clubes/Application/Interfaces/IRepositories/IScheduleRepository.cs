using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Interfaces.IRepositories
{
    public interface IScheduleRepository
    {
        Task<IEnumerable<Schedule>> GetAllAsync(CancellationToken cancellationToken);
        Task<IEnumerable<Schedule>> GetByCourtIdAsync(Guid courtId, CancellationToken cancellationToken);
        Task<Schedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<Schedule>> GetByCourtAndDateAsync(Guid courtId, DateOnly date, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
        Task AddAsync(Schedule schedule, CancellationToken cancellationToken);
        Task<IEnumerable<Schedule>> GetByCourtAndDaysOfWeekAsync(Guid courtId, List<DayOfWeek> daysOfWeek, CancellationToken cancellationToken);
        Task AddRangeAsync(IEnumerable<Schedule> schedules, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
        void Update(Schedule schedule);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> IsOwnedByUserAsync(Guid Id, Guid userId, CancellationToken cancellationToken);
    }
}