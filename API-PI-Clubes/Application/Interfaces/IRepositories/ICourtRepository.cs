using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Interfaces.IRepositories
{
    public interface ICourtRepository
    {
        Task<IEnumerable<Court>> GetAllAsync();
        Task<Court?> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task AddAsync(Court Court);
        Task SaveChangesAsync();
        void Update(Court Court);
        Task DeleteAsync(Guid id);
    }
}
