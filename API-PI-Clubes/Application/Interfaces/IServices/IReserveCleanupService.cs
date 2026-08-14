namespace API_PI_Clubes.Application.Interfaces.IServices;

public interface IReserveCleanupService
{
    Task<int> CleanupOldReservesAsync();
}