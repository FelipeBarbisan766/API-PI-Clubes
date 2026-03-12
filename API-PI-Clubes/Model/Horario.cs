namespace API_PI_Clubes.Model
{
    public class Horario : BaseEntity
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public bool IsBlocked { get; set; }
        public bool IsReserved { get; set; }
        public bool IsFixed { get; set; }
        public DayOfWeek DayOfWeek { get; set; }

        public Guid QuadraId { get; set; }
        public Quadra Quadra { get; set; }

    }
}
