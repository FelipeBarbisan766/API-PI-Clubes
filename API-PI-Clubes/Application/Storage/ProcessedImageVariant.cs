using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Application.Storage;
public sealed class ProcessedImageVariant : IDisposable
{
    public MemoryStream Stream      { get; }
    public string       FileName    { get; }
    public ImageVariantType Variant { get; }

    public ProcessedImageVariant(MemoryStream stream, string fileName, ImageVariantType variant)
    {
        Stream   = stream;
        FileName = fileName;
        Variant  = variant;
    }

    public void Dispose() => Stream.Dispose();
}