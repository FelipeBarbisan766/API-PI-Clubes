using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Application.DTOs
{
    public class CreatReserveDTO
    {
        public DateTime Date { get; set; }

        public Guid ScheduleId { get; set; }
        public Guid PlayerId { get; set; }
    }
    public class UpdateReserveDTO
    {
        public DateTime Date { get; set; }
        public StatusEnum Status { get; set; }
    }

    public class ResponseReserveDTO
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public StatusEnum Status { get; set; }
    }
}
