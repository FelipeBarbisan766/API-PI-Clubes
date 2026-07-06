using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Application.DTOs
{
    public class CreatScheduleDTO
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public DayOfWeek DayOfWeek { get; set; }
        public Guid CourtId { get; set; }
    }

    public class UpdateScheduleDTO
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public StateEnum State { get; set; }
        public DayOfWeek DayOfWeek { get; set; }

    }

    public class ResponseScheduleDTO
    {
        public Guid Id { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public StateEnum State { get; set; }
        public DayOfWeek DayOfWeek { get; set; }

        public Guid CourtId { get; set; }
    }
    public class ResponseScheduleAvailabilityDTO
    {
        public Guid Id { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public StateEnum State { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
 
        public bool IsAvailable { get; set; }
 
        public ResponseReserveInfoDTO? Reserve { get; set; }
    }
    public class ResponseReserveInfoDTO
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public StatusEnum Status { get; set; }
        public Guid PlayerId { get; set; }
    }
    
    public class ScheduleReserveDTO
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public CourtReserveDTO Court { get; set; } = null!;
    }

    public class CreateBulkScheduleDTO
    {
        public Guid CourtId { get; set; }
        public List<DayOfWeek> DaysOfWeek { get; set; } = [];
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int SlotDurationMinutes { get; set; }
    }

    public class ResponseBulkScheduleDTO
    {
        public List<ResponseScheduleDTO> Created { get; set; } = [];
        public List<ScheduleConflictDTO> Conflicts { get; set; } = [];
    }

    public class ScheduleConflictDTO
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

}
