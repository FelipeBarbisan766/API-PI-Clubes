using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Model.DTOs
{
    public class CreatQuadraDTO
    {
        public string Name { get; set; }
        public TypeEnum Type { get; set; }
        public SurfaceEnum Surface { get; set; }
        public bool IsCovered { get; set; }
        public double PricePerHour { get; set; }
        public string Description { get; set; }
        public Guid ClubeId { get; set; }
    }

    public class UpdateQuadraDTO
    {
        public string Name { get; set; }
        public TypeEnum Type { get; set; }
        public SurfaceEnum Surface { get; set; }
        public bool IsCovered { get; set; }
        public double PricePerHour { get; set; }
        public string Description { get; set; }
    }

    public class QuadraResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public TypeEnum Type { get; set; }
        public SurfaceEnum Surface { get; set; }
        public bool IsCovered { get; set; }
        public double PricePerHour { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
    }
}
