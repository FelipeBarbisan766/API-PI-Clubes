using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Application.DTOs
{
    public class CreatReserveDTO
    {
        //public string Name { get; set; }
        //public string PhoneNumber { get; set; }
        //public string Email { get; set; }
        public DateTime Date { get; set; }
        public StatusEnum Status { get; set; }

        public Guid ScheduleId { get; set; }
    }
    public class UpdateReserveDTO
    {
        //public string Name { get; set; }
        //public string PhoneNumber { get; set; }
        //public string Email { get; set; }
        public DateTime Date { get; set; }
        public StatusEnum Status { get; set; }
    }

    public class ResponseReserveDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime Date { get; set; }
        public StatusEnum Status { get; set; }
    }
}
