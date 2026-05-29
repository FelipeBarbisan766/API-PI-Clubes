using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Model
{
    public class Admin : BaseEntity
    {
        public TypeAccessEnum TypeAccess { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public virtual ICollection <ClubAdmin> ClubAdmin { get; set; }
        public ICollection<Subscription> Subscriptions { get; set; } = [];

    }
}
