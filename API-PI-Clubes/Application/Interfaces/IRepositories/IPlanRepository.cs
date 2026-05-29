using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Interfaces.IRepositories
{
    public interface IPlanRepository
    {
        Task SaveChangesAsync();
    }
}
