using API_PI_Clubes.Model;
using MimeKit;

namespace API_PI_Clubes.Application.Interfaces.IRepositories
{
    public interface IImageRepository
    {
        Task<Image> GetByBlobNameAsync(string blobName);
        void Remove(Image image);
        Task<bool> SaveChangesAsync();
    }
}
