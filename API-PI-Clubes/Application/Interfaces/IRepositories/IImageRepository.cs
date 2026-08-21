using API_PI_Clubes.Model;
using MimeKit;

namespace API_PI_Clubes.Application.Interfaces.IRepositories
{
    public interface IImageRepository
    {
        Task<Image> GetByNameAsync(string fileName);
        void Remove(Image image);
        void Add(Image image);
        Task<bool> SaveChangesAsync();
        Task<bool> IsOwnedByUserAsync(Guid Id, Guid userId);
    }
}
