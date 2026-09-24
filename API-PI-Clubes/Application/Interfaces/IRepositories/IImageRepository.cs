using API_PI_Clubes.Model;
using MimeKit;

namespace API_PI_Clubes.Application.Interfaces.IRepositories
{
    public interface IImageRepository
    {
        Task<Image> GetByNameAsync(string fileName, CancellationToken cancellationToken);
        void Remove(Image image);
        void Add(Image image);
        Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
        Task<bool> IsOwnedByUserAsync(Guid Id, Guid userId, CancellationToken cancellationToken);
    }
}
