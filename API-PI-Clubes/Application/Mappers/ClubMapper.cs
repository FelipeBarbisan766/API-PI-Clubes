using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IMappers;
using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Mappers
{
    public class ClubMapper : IClubMapper
    {
        public ResponseClubDTO ToDTO(Club club)
        {
            return new ResponseClubDTO
            {
                Id = club.Id,
                Name = club.Name,
                PhoneNumber = club.PhoneNumber,
                ZipCode = club.Address.ZipCode,
                Street = club.Address.Street,
                Number = club.Address.Number,
                Neighborhood = club.Address.Neighborhood,
                Complement = club.Address.Complement,
                City = club.Address.City,
                State = club.Address.State,
                Country = club.Address.Country,
                Description = club.Description,
                MinPrice = club.Courts
                    .Where(co => co.IsActive)
                    .Min(co => (decimal?)co.PricePerHour) ?? 0,
                CourtCount = club.Courts
                    .Count(co => co.IsActive),
                Types = club.Courts
                    .Where(co => co.IsActive)
                    .Select(co => co.Type)
                    .Distinct()
                    .ToList()
            };
        }

        public IEnumerable<ResponseClubDTO> ToDTO(IEnumerable<Club> clubs)
        {
            return clubs.Select(ToDTO);
        }

        public ResponseClubByIdDTO ToDTOById(Club club)
        {
            return new ResponseClubByIdDTO
            {
                Name = club.Name,
                PhoneNumber = club.PhoneNumber,
                ZipCode = club.Address.ZipCode,
                Street = club.Address.Street,
                Number = club.Address.Number,
                Neighborhood = club.Address.Neighborhood,
                Complement = club.Address.Complement,
                City = club.Address.City,
                State = club.Address.State,
                Country = club.Address.Country,
                Description = club.Description,
                Courts = club.Courts
                    .Where(co => co.IsActive)
                    .Select(q => new ResponseCourtDTO
                    {
                        Id = q.Id,
                        Name = q.Name,
                        Type = q.Type,
                        Surface = q.Surface,
                        IsCovered = q.IsCovered,
                        PricePerHour = q.PricePerHour
                    }).ToList()
            };
        }
    }
}