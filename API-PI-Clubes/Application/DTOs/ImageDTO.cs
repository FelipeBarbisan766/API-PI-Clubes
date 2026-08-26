namespace API_PI_Clubes.Application.DTOs
{
    public class ImageDTO
    {
        public Guid Id { get; set; }
        public string ThumbUrl  { get; set; } = string.Empty;
        public string MediumUrl { get; set; } = string.Empty;
        public string FullUrl   { get; set; } = string.Empty;
        public int Order { get; set; }
    }
    public class UploadImageDTO
    {
        public List<IFormFile> Images { get; set; }
    }
    public class DeleteImageDto
    {
        public List<Guid> ImageIds { get; set; }
    }
}
