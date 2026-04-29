using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace API_PI_Clubes.Application.Interfaces.IRepositories
{
    public interface IAdminRepository
    {
        Task<Admin?> GetByUserIdAsync(Guid id);
        Task<Admin?> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task AddAsync(Admin Admin);
        Task SaveChangesAsync();
        void Update(Admin Admin);
        Task DeleteAsync(Guid id);
        IExecutionStrategy CreateExecutionStrategy();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
