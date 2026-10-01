using API_PI_Clubes.Application.Common;
using API_PI_Clubes.Application.Interfaces.IServices;
using Microsoft.Extensions.Options;

namespace API_PI_Clubes.Application.Services;

public class BookingPolicy : IBookingPolicy
{
    private readonly TimeProvider _timeProvider;
    private readonly BookingOptions _options;
    private readonly TimeZoneInfo _timeZone;

    public BookingPolicy(TimeProvider timeProvider, IOptions<BookingOptions> options)
    {
        _timeProvider = timeProvider;
        _options = options.Value;
        _timeZone = TimeZoneInfo.FindSystemTimeZoneById(_options.TimeZoneId);
    }

    public DateTime GetEarliestBookable()
    {
        var localNow = TimeZoneInfo.ConvertTime(_timeProvider.GetUtcNow(), _timeZone).DateTime;
        return localNow.AddMinutes(_options.MinAdvanceMinutes);
    }
    
    public bool IsBookable(DateTime date, TimeOnly startTime)
        => date.Date.Add(startTime.ToTimeSpan()) >= GetEarliestBookable();
}