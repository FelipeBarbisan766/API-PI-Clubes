namespace API_PI_Clubes.Application.Storage;

public class LocalStorageService : IStorageService
{
    private readonly string _storagePath;
    private readonly string _baseUrl;
 
    public LocalStorageService(IConfiguration configuration, IWebHostEnvironment env)
    {
        var rootPath = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
        _storagePath = Path.Combine(rootPath, "uploads");
        _baseUrl = configuration.GetSection("LocalStorage:BaseUrl").Value
                   ?? throw new InvalidOperationException("LocalStorage:BaseUrl não configurado no appsettings.");
 
        Directory.CreateDirectory(_storagePath);
    }
 
    public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
    {
        var filePath = Path.Combine(_storagePath, fileName);
 
        using var outputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await fileStream.CopyToAsync(outputStream);
 
        return $"{_baseUrl.TrimEnd('/')}/uploads/{fileName}";
    }
 
    public async Task<bool> DeleteFileAsync(string blobName)
    {
        if (string.IsNullOrWhiteSpace(blobName))
            throw new InvalidOperationException("blobName is null or empty");
 
        return await DeleteBlobAsync(blobName);
    }
 
    public Task<bool> DeleteBlobAsync(string blobName)
    {
        var filePath = Path.Combine(_storagePath, blobName);
 
        if (!File.Exists(filePath))
            return Task.FromResult(false);
 
        File.Delete(filePath);
        return Task.FromResult(true);
    }
}

