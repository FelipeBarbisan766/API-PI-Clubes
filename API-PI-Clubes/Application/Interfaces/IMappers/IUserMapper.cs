using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Interfaces.IMappers
{
    public interface IUserMapper
    {
        ResponseUserDTO ToDTO(User user);
        IEnumerable<ResponseUserDTO> ToDTO(IEnumerable<User> users);
    }
}
