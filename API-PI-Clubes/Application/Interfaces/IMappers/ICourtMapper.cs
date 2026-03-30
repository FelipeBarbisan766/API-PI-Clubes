using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Interfaces.IMappers
{
    public interface ICourtMapper
    {
        ResponseCourtDTO ToDTO(Court court);
        IEnumerable<ResponseCourtDTO> ToDTO(IEnumerable<Court> courts);
    }
}
