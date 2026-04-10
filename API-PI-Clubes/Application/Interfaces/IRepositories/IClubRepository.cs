using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Interfaces.IRepositories
{
    public interface IClubRepository
    {
        Task<IEnumerable<ResponseClubDTO>> GetAllAsync();
        Task<Club> GetByIdAsync(Guid id);
        Task AddAsync(Club entity);
        Task AddClubAdminAsync(ClubAdmin clubAdmin);
        void Update(Club entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task SaveChangesAsync();
    }
}
