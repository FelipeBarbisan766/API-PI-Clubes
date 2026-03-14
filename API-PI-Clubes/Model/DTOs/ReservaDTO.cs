using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Model.DTOs
{
    public class CreatReservaDTO
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime Date { get; set; }
        public StatusEnum Status { get; set; }

        public Guid HorarioId { get; set; }
    }
    public class UpdateReservaDTO
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime Date { get; set; }
        public StatusEnum Status { get; set; }
    }

    public class ResponseReservaDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime Date { get; set; }
        public StatusEnum Status { get; set; }
    }
}
