using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IMappers;
using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Mappers
{
    public class UserMapper : IUserMapper
    {
        public ResponseUserDTO ToDTO(User user)
        {
            return new ResponseUserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
            };
        }

        public IEnumerable<ResponseUserDTO> ToDTO(IEnumerable<User> users)
        {
            return users.Select(ToDTO);
        }
    }
}
