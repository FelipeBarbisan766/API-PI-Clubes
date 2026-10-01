namespace API_PI_Clubes.Application.Common;

public class BookingOptions
{
    public const string SectionName = "Booking";
    public int MinAdvanceMinutes { get; set; } = 10;
    public string TimeZoneId { get; set; } = "America/Sao_Paulo";
}