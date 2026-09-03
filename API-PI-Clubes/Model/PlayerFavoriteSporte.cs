namespace API_PI_Clubes.Model
{
    public class PlayerFavoriteSport
    {
        public Guid PlayerId { get; set; }
        public virtual Player Player { get; set; }

        public Guid SportId { get; set; }
        public virtual Sport Sport { get; set; }
    }
}