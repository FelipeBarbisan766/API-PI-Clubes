using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Model
{
    public class Player : BaseEntity
    {
        public RankCategoryEnum RankCategory { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public virtual ICollection<Reserve> Reserves { get; set; }
    }
}
