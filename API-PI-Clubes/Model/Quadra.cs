using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Model
{
    public class Quadra : BaseEntity
    {
        //public Quadra() { }

        public string Name { get; set; }
        public TypeEnum Type { get; set; }
        public SurfaceEnum Surface { get; set; }
        public bool IsCovered { get; set; }
        public double PricePerHour { get; set; }
        public string Description { get; set; }

        public Guid ClubeId { get; set; }
        public Clube Clube { get; set; }

        public ICollection<Horario> Horarios { get; set; }

    }
}
