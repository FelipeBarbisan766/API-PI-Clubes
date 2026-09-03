using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IMappers;
using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Mappers
{
    public class CourtMapper : ICourtMapper
    {
        private static ImageDTO ToImageDTO(Image i) => new()
        {
            Id        = i.Id,
            ThumbUrl  = i.ThumbUrl,
            MediumUrl = i.MediumUrl,
            FullUrl   = i.FullUrl,
            Order     = i.Order
        };

        public ResponseCourtDTO ToDTO(Court court)
        {
            return new ResponseCourtDTO
            {
                Id = court.Id,
                Name = court.Name,
                Surface = court.Surface,
                IsCovered = court.IsCovered,
                PricePerHour = court.PricePerHour,
                Description = court.Description,
                ClubId = court.ClubId,
                Sports = court.CourtSports?
                    .Select(cs => new SportDTO { Id = cs.Sport.Id, Name = cs.Sport.Name })
                    .ToList() ?? new List<SportDTO>(),
                Images = court.Images
                    .Select(ToImageDTO)
                    .ToList()
            };
        }

        public IEnumerable<ResponseCourtDTO> ToDTO(IEnumerable<Court> courts)
        {
            return courts.Select(ToDTO);
        }
    }
}
