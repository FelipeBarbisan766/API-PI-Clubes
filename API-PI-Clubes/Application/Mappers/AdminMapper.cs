using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IMappers;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Application.Mappers
{
    public class AdminMapper : IAdminMapper
    {
        public ResponseAdminDTO ToDTO(Admin admin)
        {
            return new ResponseAdminDTO
            {
                Id = admin.Id,
                UserName = admin.UserName,
                ContactNumber = admin.ContactNumber,
                Description = admin.Description,
                TypeAccess = admin.TypeAccess,
                UserId = admin.UserId

            };
        }

        public IEnumerable<ResponseAdminDTO> ToDTO(IEnumerable<Admin> admins)
        {
            return admins.Select(ToDTO);
        }
    }
}
