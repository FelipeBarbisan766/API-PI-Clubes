namespace API_PI_Clubes.Application.DTOs
{
    public class ShareClubDataDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public decimal? MinPrice { get; set; }
        public List<string> Sports { get; set; } = new();
        public Guid? CoverImageId { get; set; }
        public string? CoverUrl { get; set; }
    }
}
