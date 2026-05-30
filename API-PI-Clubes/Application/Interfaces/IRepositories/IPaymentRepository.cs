using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Interfaces.IRepositories
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(Guid id);
        Task<Payment?> GetByGatewayTransactionIdAsync(string gatewayTransactionId);
        Task<IEnumerable<Payment>> GetByAdminIdAsync(Guid adminId);
        Task AddAsync(Payment payment);
        Task UpdateAsync(Payment payment);

    }
}
