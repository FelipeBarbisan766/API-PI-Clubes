
namespace API_PI_Clubes.Application.Storage;

public interface IImageProcessingService
{
    Task<ProcessedImageResult> ProcessAsync(Stream inputStream);
}