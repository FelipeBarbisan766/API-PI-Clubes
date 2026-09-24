using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Interfaces.IRepositories
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(Guid id,CancellationToken cancellationToken);
        Task<IEnumerable<Payment>> GetByAdminIdAsync(Guid adminId,CancellationToken cancellationToken);
        Task AddAsync(Payment payment);
        Task UpdateAsync(Payment payment);
    }
}