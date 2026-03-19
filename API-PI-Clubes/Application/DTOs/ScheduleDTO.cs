using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Application.DTOs
{
    public class CreatScheduleDTO
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public bool IsBlocked { get; set; }
        public bool IsReserved { get; set; }
        public bool IsFixed { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public Guid CourtId { get; set; }
    }

    public class UpdateScheduleDTO
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsReserved { get; set; }
        public bool IsFixed { get; set; }
        public DayOfWeek DayOfWeek { get; set; }

    }

    public class ResponseScheduleDTO
    {
        public Guid Id { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsReserved { get; set; }
        public bool IsFixed { get; set; }
        public DayOfWeek DayOfWeek { get; set; }

        public Guid CourtId { get; set; }
    }
}
