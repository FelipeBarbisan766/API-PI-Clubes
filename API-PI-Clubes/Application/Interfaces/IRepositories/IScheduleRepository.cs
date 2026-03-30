using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Interfaces.IRepositories
{
    public interface IScheduleRepository
    {
        Task<IEnumerable<Schedule>> GetAllAsync();
        Task<IEnumerable<Schedule>> GetByCourtIdAsync(Guid courtId);
        Task<Schedule?> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task AddAsync(Schedule schedule);
        Task SaveChangesAsync();
        void Update(Schedule schedule);
        Task DeleteAsync(Guid id);
    }
}