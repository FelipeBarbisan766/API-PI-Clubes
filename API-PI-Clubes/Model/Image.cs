namespace API_PI_Clubes.Model
{
    public class Image : BaseEntity
    {
        public string Name { get; set; }

        public string ThumbUrl  { get; set; } = string.Empty;
        public string MediumUrl { get; set; } = string.Empty;
        public string FullUrl   { get; set; } = string.Empty;
        
        public Guid? ClubId { get; set; }
        public virtual Club Club { get; set; }

        public Guid? CourtId { get; set; }
        public virtual Court Court { get; set; }
    }
}
