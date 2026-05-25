using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Model
{
    public class Reserve : BaseEntity
    {
        public DateTime Date { get; set; }
        public StatusEnum Status { get; set; }

        public Guid ScheduleId { get; set; }
        public virtual Schedule Schedule { get; set; }

        public Guid PlayerId { get; set; }
        public virtual Player Player { get; set; }
    }
}
