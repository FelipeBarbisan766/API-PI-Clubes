using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace API_PI_Clubes.Application.Interfaces.IRepositories
{
    public interface IAdminRepository
    {
        Task<Admin?> GetByUserIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Admin?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
        Task AddAsync(Admin Admin, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
        void Update(Admin Admin);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        IExecutionStrategy CreateExecutionStrategy();
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
        Task<bool> IsOwnedByUserAsync(Guid Id, Guid userId, CancellationToken cancellationToken);
    }
}
