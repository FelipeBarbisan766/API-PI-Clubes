using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Interfaces.IMappers
{
    public interface IClubMapper
    {
        ResponseClubDTO ToDTO(Club club);
        IEnumerable<ResponseClubDTO> ToDTO(IEnumerable<Club> clubs);
    }
}
