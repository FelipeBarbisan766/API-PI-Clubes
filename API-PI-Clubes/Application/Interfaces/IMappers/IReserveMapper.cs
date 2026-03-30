using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Interfaces.IMappers
{
    public interface IReserveMapper
    {
        ResponseReserveDTO ToDTO(Reserve reserve);
        IEnumerable<ResponseReserveDTO> ToDTO(IEnumerable<Reserve> reserves);
    }
}
