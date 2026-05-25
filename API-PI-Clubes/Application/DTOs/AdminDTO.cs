using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Application.DTOs
{
    public class CreatAdminDTO
    {
        public Guid UserId { get; set; }
    }

    public class UpdateAdminDTO
    {

    }

    public class ResponseAdminDTO
    {
        public Guid Id { get; set; }
        public TypeAccessEnum TypeAccess { get; set; }
        public Guid UserId { get; set; }
    }
}
