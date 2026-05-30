namespace API_PI_Clubes.Model
{
    public class Subscription : BaseEntity
        {
            // public TypeAccess TypeAccess { get; set; }
            public DateTime StartDate { get; set; } = DateTime.UtcNow;
            public DateTime ExpiresAt { get; set; }
     
            public Guid AdminId { get; set; }
            public Guid PlanId { get; set; }
            public Guid PaymentId { get; set; }
     
            public Admin Admin { get; set; } = null!;
            public Plan Plan { get; set; } = null!;
            public Payment Payment { get; set; } = null!;
        }
}
