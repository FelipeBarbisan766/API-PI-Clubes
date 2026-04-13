namespace API_PI_Clubes.Application.DTOs
{
    public class UploadImageDTO
    {
        public List<IFormFile> Images { get; set; }
    }
    public class DeleteImageDto
    {
        public List<Guid> ImageIds { get; set; }
    }
}
