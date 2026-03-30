using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IMappers;
using API_PI_Clubes.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                Description = club.Description
            };
        }

        public IEnumerable<ResponseClubDTO> ToDTO(IEnumerable<Club> clubs)
        {
            return clubs.Select(ToDTO);
        }
    }
}
