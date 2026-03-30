using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Interfaces.IMappers
{
    public interface IAdminMapper
    {
        ResponseAdminDTO ToDTO(Admin admin);
        IEnumerable<ResponseAdminDTO> ToDTO(IEnumerable<Admin> admins);
    }
}
