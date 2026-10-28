namespace API_PI_Clubes.Model
{
    public class Plan : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int QuantClub { get; set; }
        public int QuantCourt { get; set; }
        public int DurationDays { get; set; }
        public bool IsCustom { get; set; }
        public Guid? OwnerAdminId { get; set; }
        public Admin? OwnerAdmin { get; set; }
        public ICollection<Subscription> Subscriptions { get; set; } = [];
    }
}