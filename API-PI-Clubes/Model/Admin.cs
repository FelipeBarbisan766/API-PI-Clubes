using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Model
{
    public class Admin : BaseEntity
    {
        public string UserName { get; set; }
        public string ContactNumber { get; set; }
        public string Description { get; set; }
        public TypeAccessEnum TypeAccess { get; set; }

        public Guid ClubId { get; set; }
        public Club Club { get; set; } 

        public Guid UserId { get; set; }
        public User User { get; set; }

    }
}
