namespace API_PI_Clubes.Model.DTOs
{
   
    public class CreateClubeDTO
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
    }

    public class UpdateClubeDTO
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
    }
    public class ClubeResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public List<QuadraResponseDTO> Quadras { get; set; } = new();
    }


}
