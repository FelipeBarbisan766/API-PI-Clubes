using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Interfaces.IRepositories
{
    public interface IAdminRepository
    {
        Task<IEnumerable<Admin>> GetAllAsync();
        Task<Admin?> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task AddAsync(Admin Admin);
        Task SaveChangesAsync();
        void Update(Admin Admin);
        Task DeleteAsync(Guid id);
        Task<IDisposable> BeginTransactionAsync();
    }
}
