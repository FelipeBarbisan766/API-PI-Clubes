namespace API_PI_Clubes.Model
{
    public class ClubAdmin : BaseEntity
    {
        public Guid AdminId { get; set; }
        public virtual Admin Admin { get; set; }

        public Guid ClubId { get; set; }
        public virtual Club Club { get; set; }
    }
}
