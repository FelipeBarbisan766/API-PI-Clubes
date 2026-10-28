namespace API_PI_Clubes.Application.Interfaces.IRepositories
{
    public interface IUnitOfWork
    {
        Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken);
    }
}