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
    public class ResponseScheduleAvailabilityDTO
    {
        public Guid Id { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsFixed { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
 
        /// <summary>
        /// Verdadeiro quando o horário não está bloqueado e não há reserva ativa para a data consultada.
        /// </summary>
        public bool IsAvailable { get; set; }
 
        /// <summary>
        /// Nulo quando não há reserva para a data consultada.
        /// </summary>
        public ResponseReserveInfoDTO? Reserve { get; set; }
    }
    public class ResponseReserveInfoDTO
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public StatusEnum Status { get; set; }
        public Guid PlayerId { get; set; }
    }

}
