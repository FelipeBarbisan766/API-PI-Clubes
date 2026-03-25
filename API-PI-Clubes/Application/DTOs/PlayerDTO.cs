using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Application.DTOs
{
    public class CreatPlayerDTO
    {
        public string UserName { get; set; }
        public string ContactNumber { get; set; }
        public string Description { get; set; }

        public Guid UserId { get; set; }
    }

    public class UpdatePlayerDTO
    {
        public string UserName { get; set; }
        public string ContactNumber { get; set; }
        public string Description { get; set; }

    }

    public class ResponsePlayerDTO
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string ContactNumber { get; set; }
        public string Description { get; set; }
        public RankCategoryEnum RankCategory { get; set; }
        public Guid UserId { get; set; }
    }
}
