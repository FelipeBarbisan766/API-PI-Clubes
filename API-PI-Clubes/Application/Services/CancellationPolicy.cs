using API_PI_Clubes.Application.Common;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Model.Enums;
using Microsoft.Extensions.Options;

namespace API_PI_Clubes.Application.Services;

public class CancellationPolicy : ICancellationPolicy
{
    private readonly TimeProvider _timeProvider;
    private readonly BookingOptions _options;
    private readonly TimeZoneInfo _timeZone;

    public CancellationPolicy(TimeProvider timeProvider, IOptions<BookingOptions> options)
    {
        _timeProvider = timeProvider;
        _options = options.Value;
        _timeZone = TimeZoneInfo.FindSystemTimeZoneById(_options.TimeZoneId);
    }

    public DateTime GetDeadline(DateTime date, TimeOnly startTime)
        => date.Date.Add(startTime.ToTimeSpan()).AddMinutes(-_options.CancellationCutoffMinutes);

    public bool CanCancel(DateTime date, TimeOnly startTime, StatusEnum currentStatus)
        => currentStatus != StatusEnum.Cancelada
           && NowLocal() < GetDeadline(date, startTime);

    private DateTime NowLocal()
        => TimeZoneInfo.ConvertTime(_timeProvider.GetUtcNow(), _timeZone).DateTime;
}