using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Application.Interfaces.IServices;

public interface ICancellationPolicy
{
    bool CanCancel(DateTime date, TimeOnly startTime, StatusEnum currentStatus);
    DateTime GetDeadline(DateTime date, TimeOnly startTime);
}