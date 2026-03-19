using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Application.DTOs
{
    public class CreatAdminDTO
    {
        public string UserName { get; set; }
        public string ContactNumber { get; set; }
        public string Description { get; set; }
        public TypeAccessEnum TypeAccess { get; set; }

        public Guid UserId { get; set; }
    }

    public class UpdateAdminDTO
    {
        public string UserName { get; set; }
        public string ContactNumber { get; set; }
        public string Description { get; set; }
        public TypeAccessEnum TypeAccess { get; set; }

    }

    public class ResponseAdminDTO
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string ContactNumber { get; set; }
        public string Description { get; set; }
        public TypeAccessEnum TypeAccess { get; set; }
        public Guid UserId { get; set; }
    }
}
