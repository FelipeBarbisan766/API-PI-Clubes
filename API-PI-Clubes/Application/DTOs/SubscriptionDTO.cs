using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Application.DTOs
{
    public record CreateSubscriptionDto(
        Guid AdminId,
        Guid PlanId,
        Guid PaymentId
    );
 
    public record SubscriptionResponseDto(
        Guid Id,
        Guid AdminId,
        Guid PlanId,
        string PlanName,
        DateTime StartDate,
        DateTime ExpiresAt,
        bool IsActive
    );

}
