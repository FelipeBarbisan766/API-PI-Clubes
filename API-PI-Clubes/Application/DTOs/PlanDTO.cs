using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Application.DTOs
{
    public record CreatePlanDto(
        string Name,
        string Description,
        decimal Price,
        int QuantClub,
        int QuantCourt,
        int DurationDays
    );
 
    public record UpdatePlanDto(
        string? Name,
        string? Description,
        decimal? Price,
        int? QuantClub,
        int? QuantCourt,
        int? DurationDays
    );
 
    public record PlanResponseDto(
        Guid Id,
        string Name,
        string Description,
        decimal Price,
        int QuantClub,
        int QuantCourt,
        int DurationDays,
        bool IsActive
    );
    public class PlanLimitsDTO
    {
        public string PlanName { get; set; } = string.Empty;
        public int QuantClub { get; set; }
        public int QuantCourt { get; set; }
    }

    public class PlanUsageDTO
    {
        public string PlanName { get; set; } = string.Empty;
        public ResourceUsageDTO Clubs { get; set; } = new();
        public int CourtLimitPerClub { get; set; }
        public List<ClubCourtUsageDTO> ClubsCourtUsage { get; set; } = new();
    }

    public class ResourceUsageDTO
    {
        public int Used { get; set; }
        public int Limit { get; set; }
    }

    public class ClubCourtUsageDTO
    {
        public Guid ClubId { get; set; }
        public string ClubName { get; set; } = string.Empty;
        public int Used { get; set; }
        public int Limit { get; set; }
    }
}
