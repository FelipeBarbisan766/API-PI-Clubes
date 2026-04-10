namespace API_PI_Clubes.Application.DTOs
{
    public class UploadImageDTO
    {
        public Guid? clubId { get; set; }
        public Guid? courtId { get; set; }
        public List<IFormFile> Images { get; set; }
    }
}
