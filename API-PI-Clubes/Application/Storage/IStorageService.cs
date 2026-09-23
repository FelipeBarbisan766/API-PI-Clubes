namespace API_PI_Clubes.Application.Storage
{
    public interface IStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName,CancellationToken cancellationToken);
        Task<bool> DeleteFileAsync(string fileName, CancellationToken cancellationToken);
    }
}
