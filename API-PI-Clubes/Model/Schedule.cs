namespace API_PI_Clubes.Model
{
    public class Schedule : BaseEntity
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public bool IsBlocked { get; set; }
        public bool IsReserved { get; set; }
        public bool IsFixed { get; set; }
        public DayOfWeek DayOfWeek { get; set; }

        public Guid CourtId { get; set; }
        public Court Court { get; set; }

        public ICollection<Reserve> Reserves { get; set; }

    }
}
