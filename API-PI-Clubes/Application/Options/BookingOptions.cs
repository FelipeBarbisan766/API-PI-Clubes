namespace API_PI_Clubes.Application.Common;

public class BookingOptions
{
    public const string SectionName = "Booking";
    public int BookingCutoffMinutes { get; set; } = 10;
    public int CancellationCutoffMinutes { get; set; } = 0; 
    public string TimeZoneId { get; set; } = "America/Sao_Paulo";
}