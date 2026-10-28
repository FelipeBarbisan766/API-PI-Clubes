using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Model;

public class PlanPricing
{
    public Guid Id { get; set; }
    public PricingComponent Component { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime UpdatedAt { get; set; }
}