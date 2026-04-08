namespace API_PI_Clubes.Model
{
    public class Image : BaseEntity
    {
        public string Name { get; set; }
        public string Url { get; set; }


        public Guid? ClubId { get; set; }
        public virtual Club Club { get; set; }

        public Guid? CourtId { get; set; }
        public virtual Court Court { get; set; }
    }
}
