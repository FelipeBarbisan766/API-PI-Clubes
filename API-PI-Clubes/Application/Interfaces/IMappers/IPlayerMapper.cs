using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Interfaces.IMappers
{
    public interface IPlayerMapper
    {
        ResponsePlayerDTO ToDTO(Player player);
        IEnumerable<ResponsePlayerDTO> ToDTO(IEnumerable<Player> players);
    }
}
