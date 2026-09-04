using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IRepositories
{
    public interface ISportRepository
    {
        Task<int> CountExistingAsync(List<Guid> ids);
        Task<List<SportDTO>> GetAllAsync();
    }
}