using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface ISportService
    {
        Task<List<SportDTO>> GetAll();
    }
}