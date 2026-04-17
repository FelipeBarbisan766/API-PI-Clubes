namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface IImageService
    {
        Task<bool> DeleteImageAsync(string blobName);
    }
}
