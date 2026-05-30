using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Model
{
    public class Payment : BaseEntity
        {
            public decimal Amount { get; set; }
            public DateTime Date { get; set; } = DateTime.UtcNow;
            public PaymentMethod Method { get; set; }
            public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
 
            public string? GatewayTransactionId { get; set; }
 
            public Subscription? Subscription { get; set; }

        }
}
