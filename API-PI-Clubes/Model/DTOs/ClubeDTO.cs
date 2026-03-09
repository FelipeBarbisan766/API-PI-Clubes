namespace API_PI_Clubes.Model.DTOs
{
   
    public class CreateClubeDTO
    {
        public string Name { get; set; }
        public int PhoneNumber { get; set; }
        public string Description { get; set; }
    }

    public class UpdateClubeDTO
    {
        public string Name { get; set; }
        public int PhoneNumber { get; set; }
        public string Description { get; set; }
    }

}
