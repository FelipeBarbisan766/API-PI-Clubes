using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Interfaces.IRepositories
{
    public interface IReserveRepository
    {
        Task<IEnumerable<Reserve>> GetAllAsync();
        Task<Reserve?> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task AddAsync(Reserve Reserve);
        Task SaveChangesAsync();
        void Update(Reserve Reserve);
        Task DeleteAsync(Guid id);
    }
}
