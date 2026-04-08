using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Model
{
    public class Court : BaseEntity
    {
        //public Quadra() { }

        public string Name { get; set; }
        public TypeEnum Type { get; set; }
        public SurfaceEnum Surface { get; set; }
        public bool IsCovered { get; set; }
        public decimal PricePerHour { get; set; }
        public string Description { get; set; }

        public Guid ClubId { get; set; }
        public virtual Club Club { get; set; }

        public virtual ICollection<Schedule> Schedules { get; set; }
        public virtual ICollection<Image> Images { get; set; }
    }
}
