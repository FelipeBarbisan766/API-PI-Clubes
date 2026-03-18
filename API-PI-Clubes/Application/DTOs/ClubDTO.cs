namespace API_PI_Clubes.Application.DTOs
{
   
    public class CreateClubDTO
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
    }

    public class UpdateClubDTO
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
    }
    public class ResponseClubDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public List<ResponseCourtDTO> Courts { get; set; } = new();
    }


}
