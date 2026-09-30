namespace API_PI_Clubes.Model;

public class FlagPlayer : BaseEntity
{
    public Guid PlayerId { get; set; }
    public Guid FlagId { get; set; }
    public Guid? ReserveId { get; set; }          
    public Guid CreatedByAdminId { get; set; }
    public string? Notes { get; set; }

    public Player Player { get; set; } = null!;
    public Flag Flag { get; set; } = null!;
    public Reserve? Reserve { get; set; }
}