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
        public double PricePerHour { get; set; }
        public string Description { get; set; }

        public Guid ClubId { get; set; }
        public Club Club { get; set; }

        public ICollection<Schedule> Schedules { get; set; }

    }
}
