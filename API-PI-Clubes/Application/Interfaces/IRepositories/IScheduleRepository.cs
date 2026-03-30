using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Interfaces.IRepositories
{
    public interface IScheduleRepository
    {
        Task<IEnumerable<Schedule>> GetAllAsync();
        Task<Schedule?> GetByIdAsync(Guid id);
        Task<IEnumerable<Schedule>> GetByCourtIdAsync(Guid courtId);
        Task AddAsync(Schedule Schedule);
        void Update(Schedule Schedule);
        Task SaveChangesAsync();
    }
}
