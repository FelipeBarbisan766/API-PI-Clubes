
using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Application.Storage;

public interface IImageProcessingService
{
    Task<ProcessedImageResult> ProcessAsync(Stream inputStream);
    Task<ProcessedImageVariant> ProcessAsync(Stream inputStream, ImageVariantType variantType);
}