using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Application.DTOs
{
    public class CreatPlayerDTO
    {
        public Guid UserId { get; set; }
    }

    public class UpdatePlayerDTO
    {
        public List<Guid> FavoriteSportIds { get; set; } = new();
    }

    public class ResponsePlayerDTO
    {
        public Guid Id { get; set; }
        public RankCategoryEnum RankCategory { get; set; }
        public Guid UserId { get; set; }
        public List<SportDTO> FavoriteSports { get; set; } = new();
    }
    

}
