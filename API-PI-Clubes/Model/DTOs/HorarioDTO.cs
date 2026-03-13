using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Model.DTOs
{
    public class CreatHorarioDTO
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public bool IsBlocked { get; set; }
        public bool IsReserved { get; set; }
        public bool IsFixed { get; set; }
        public DayOfWeek DayOfWeek { get; set; }

        public Guid QuadraId { get; set; }
    }

    public class UpdateHorarioDTO
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public bool IsBlocked { get; set; }
        public bool IsReserved { get; set; }
        public bool IsFixed { get; set; }
        public DayOfWeek DayOfWeek { get; set; }

    }

    public class ResponseHorarioDTO
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public bool IsBlocked { get; set; }
        public bool IsReserved { get; set; }
        public bool IsFixed { get; set; }
        public DayOfWeek DayOfWeek { get; set; }

        public Guid QuadraId { get; set; }
    }
}
