using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Interfaces.IRepositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task AddAsync(User user, CancellationToken cancellationToken);
        Task<bool> ExistsByCpfHashAsync(string cpfHash, CancellationToken cancellationToken);
        void Update(User user);
        Task SaveChangesAsync( CancellationToken cancellationToken);
    }
}
