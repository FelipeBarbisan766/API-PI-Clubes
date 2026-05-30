using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Interfaces.IRepositories
{
    public interface IPlanRepository
    {
        Task<Plan?> GetByIdAsync(Guid id);
        Task<IEnumerable<Plan>> GetAllActiveAsync();
        Task AddAsync(Plan plan);
        Task UpdateAsync(Plan plan);


    }
}
