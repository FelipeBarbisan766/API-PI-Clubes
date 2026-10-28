using Microsoft.EntityFrameworkCore;
using API_PI_Clubes.Infrastructure.Data;


namespace API_PI_Clubes.Infrastructure.Repositories
{
    using API_PI_Clubes.Application.Interfaces.IRepositories;

    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context; 

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action,
            CancellationToken cancellationToken)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);
                await action(cancellationToken);
                await tx.CommitAsync(cancellationToken);
            });
        }
    }
}