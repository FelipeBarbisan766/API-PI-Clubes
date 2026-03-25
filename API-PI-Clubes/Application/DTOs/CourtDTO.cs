using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Application.DTOs
{
    public class CreatCourtDTO
    {
        public string Name { get; set; }
        public TypeEnum Type { get; set; }
        public SurfaceEnum Surface { get; set; }
        public bool IsCovered { get; set; }
        public decimal PricePerHour { get; set; }
        public string Description { get; set; }
        public Guid ClubId { get; set; }
    }

    public class UpdateCourtDTO
    {
        public string Name { get; set; }
        public TypeEnum Type { get; set; }
        public SurfaceEnum Surface { get; set; }
        public bool IsCovered { get; set; }
        public decimal PricePerHour { get; set; }
        public string Description { get; set; }
    }

    public class ResponseCourtDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public TypeEnum Type { get; set; }
        public SurfaceEnum Surface { get; set; }
        public bool IsCovered { get; set; }
        public decimal PricePerHour { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
    }
}
