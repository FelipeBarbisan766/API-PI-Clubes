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

        // Cria o container caso ele não exista (opcional, mas seguro)
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        var blobClient = containerClient.GetBlobClient(fileName);

        // Define o tipo de conteúdo (ex: image/jpeg) para abrir direto no navegador
        var blobHttpHeader = new BlobHttpHeaders { ContentType = "image/jpeg" };

        await blobClient.UploadAsync(fileStream, new BlobUploadOptions { HttpHeaders = blobHttpHeader });

        return blobClient.Uri.ToString(); // Retorna a URL para salvar no seu banco SQL
    }
}