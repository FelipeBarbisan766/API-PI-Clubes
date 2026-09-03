using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Application.DTOs
{
    public class SportDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class CreatCourtDTO
    {
        public string Name { get; set; }
        public SurfaceEnum Surface { get; set; }
        public bool IsCovered { get; set; }
        public decimal PricePerHour { get; set; }
        public string Description { get; set; }
        public Guid ClubId { get; set; }
        public List<Guid> SportIds { get; set; } = new();
        public List<IFormFile>? Images { get; set; }
    }

    public class UpdateCourtDTO
    {
        public string Name { get; set; }
        public SurfaceEnum Surface { get; set; }
        public bool IsCovered { get; set; }
        public decimal PricePerHour { get; set; }
        public string Description { get; set; }
        public List<Guid> SportIds { get; set; } = new();
    }

    public class ResponseCourtDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public SurfaceEnum Surface { get; set; }
        public bool IsCovered { get; set; }
        public decimal PricePerHour { get; set; }
        public string Description { get; set; }
        public Guid ClubId { get; set; }
        public List<SportDTO> Sports { get; set; } = new();
        public List<ImageDTO> Images { get; set; }
    }

    public class CourtReserveDTO
    {
        public string Name { get; set; } = string.Empty;
        public decimal PricePerHour { get; set; }
        public List<SportDTO> Sports { get; set; } = new();
    }

    public class CourtQueryDTO
    {
        public string? Name { get; set; }
        public string? City { get; set; }
        public List<Guid>? SportIds { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}