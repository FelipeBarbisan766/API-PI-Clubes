using API_PI_Clubes.Application.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

public class AzureStorageService : IStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public AzureStorageService(BlobServiceClient blobServiceClient, IConfiguration configuration)
    {
        _blobServiceClient = blobServiceClient;
        _containerName = configuration.GetSection("AzureStorage:ContainerName").Value;
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        var blobClient = containerClient.GetBlobClient(fileName);

        var blobHttpHeader = new BlobHttpHeaders { ContentType = "image/jpeg" };

        await blobClient.UploadAsync(fileStream, new BlobUploadOptions { HttpHeaders = blobHttpHeader });

        return blobClient.Uri.ToString(); // Retorna a URL para salvar no seu banco SQL
    }

    public async Task<bool> DeleteFileAsync(string blobName)
    {
        if (string.IsNullOrWhiteSpace(blobName))
            throw new InvalidOperationException("blobName is null or empty");

        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        var result = await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        return result.Value;
    }
    public async Task<bool> DeleteBlobAsync(string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        return await blobClient.DeleteIfExistsAsync();
    }

}