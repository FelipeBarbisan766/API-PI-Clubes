using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Application.DTOs
{
    public class ReserveQueryDTO
    {
        public string? Name { get; set; }
        public StatusEnum? Status { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
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
    public class ResponseReserveDetailDTO
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public StatusEnum Status { get; set; }
        public string Name { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public ScheduleReserveDTO Schedule { get; set; } = null!;
    }
    public class ResponseReserveDetailToPlayerDTO
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public StatusEnum Status { get; set; }
        public ScheduleReserveDTO Schedule { get; set; } = null!;
        public ClubReserveDTO Club { get; set; } = null!;
    }

}
