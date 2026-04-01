using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Interfaces.IMappers
{
    public interface IClubMapper
    {
        IEnumerable<ResponseClubDTO> ToDTO(IEnumerable<Club> entities);
        ResponseClubDTO ToDTO(Club entity);
        ResponseClubByIdDTO ToDTOById(Club entity);
    }
}
