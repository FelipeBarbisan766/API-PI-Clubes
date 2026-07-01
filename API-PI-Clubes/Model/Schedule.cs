using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Model
{
    public class Schedule : BaseEntity
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public StateEnum State { get; set; }
        public DayOfWeek DayOfWeek { get; set; }

        public Guid CourtId { get; set; }
        public virtual Court Court { get; set; }

        public virtual ICollection<Reserve> Reserves { get; set; }

    }
}
