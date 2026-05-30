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

}
