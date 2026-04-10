using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IMappers;
using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Mappers
{
    public class CourtMapper : ICourtMapper
    {
        public ResponseCourtDTO ToDTO(Court court)
        {
            return new ResponseCourtDTO
            {
                Id = court.Id,
                Name = court.Name,
                Type = court.Type,
                Surface = court.Surface,
                IsCovered = court.IsCovered,
                PricePerHour = court.PricePerHour,
                Description = court.Description
            };
        }

        public IEnumerable<ResponseCourtDTO> ToDTO(IEnumerable<Court> courts)
        {
            return courts.Select(ToDTO);
        }
    }
}
