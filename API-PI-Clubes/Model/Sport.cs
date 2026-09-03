namespace API_PI_Clubes.Model
{
    public class Sport : BaseEntity
    {
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual ICollection<CourtSport> CourtSports { get; set; }
    }
}