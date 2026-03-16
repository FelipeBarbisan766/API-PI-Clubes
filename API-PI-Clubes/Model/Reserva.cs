using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Model
{
    public class Reserva : BaseEntity
    {
        //public string Name { get; set; }
        //public string PhoneNumber { get; set; }
        //public string Email { get; set; }
        public DateTime Date { get; set; }
        public StatusEnum Status { get; set; }

        public Guid HorarioId { get; set; }
        public Horario Horario { get; set; }

        public Guid PlayerId { get; set; }
        public Player Player { get; set; }
    }
}
