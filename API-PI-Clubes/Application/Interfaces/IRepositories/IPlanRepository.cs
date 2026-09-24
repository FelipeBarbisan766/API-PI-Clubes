using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Interfaces.IRepositories
{
    public interface IPlanRepository
    {
        Task<Plan?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<Plan>> GetAllActiveAsync(CancellationToken  cancellationToken);
        Task AddAsync(Plan plan,CancellationToken  cancellationToken);
        Task UpdateAsync(Plan plan,CancellationToken  cancellationToken);


    }
}
