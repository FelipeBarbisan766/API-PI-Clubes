using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Model
{
    public class Player : BaseEntity
    {
        public string UserName { get; set; }
        public string ContactNumber { get; set; }
        public string Description { get; set; }
        public RankCategoryEnum RankCategory { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public virtual ICollection<Reserve> Reserves { get; set; }
    }
}
