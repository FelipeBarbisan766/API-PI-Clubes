namespace API_PI_Clubes.Application.Interfaces.IServices;

public interface IBookingPolicy
{
    DateTime GetEarliestBookable();
    bool IsBookable(DateTime date, TimeOnly startTime);
}