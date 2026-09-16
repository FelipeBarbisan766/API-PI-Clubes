namespace API_PI_Clubes.Model
{
    public class ClubReview : BaseEntity
    {
        public Guid ClubId { get; set; }
        public virtual Club Club { get; set; }

        public Guid PlayerId { get; set; }
        public virtual Player Player { get; set; }

        public decimal Rating { get; set; } 
    }
}