using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IMappers;
using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Mappers
{
    public class PlayerMapper : IPlayerMapper
    {
        public ResponsePlayerDTO ToDTO(Player player)
        {
            return new ResponsePlayerDTO
            {
                Id = player.Id,
                UserName = player.UserName,
                ContactNumber = player.ContactNumber,
                Description = player.Description,
                RankCategory = player.RankCategory,
                UserId = player.UserId
            };
        }

        public IEnumerable<ResponsePlayerDTO> ToDTO(IEnumerable<Player> players)
        {
            return players.Select(ToDTO);
        }
    }
}
