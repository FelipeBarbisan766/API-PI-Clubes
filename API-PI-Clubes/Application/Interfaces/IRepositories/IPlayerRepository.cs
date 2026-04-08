using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Interfaces.IRepositories
{
    public interface IPlayerRepository
    {
        Task<IEnumerable<Player>> GetAllAsync();
        Task<Player?> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task AddAsync(Player Player);
        Task SaveChangesAsync();
        void Update(Player Player);
        Task DeleteAsync(Guid id);
        Task<IDisposable> BeginTransactionAsync();
    }
}
