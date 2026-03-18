using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces
{
    public interface ICourtService
    {
        Task<IEnumerable<ResponseCourtDTO>> GetAll();
    }
}
