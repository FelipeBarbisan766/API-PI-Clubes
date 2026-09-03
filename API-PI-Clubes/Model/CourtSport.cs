namespace API_PI_Clubes.Model
{
    public class CourtSport
    {
        public Guid CourtId { get; set; }
        public virtual Court Court { get; set; }

        public Guid SportId { get; set; }
        public virtual Sport Sport { get; set; }
    }
}