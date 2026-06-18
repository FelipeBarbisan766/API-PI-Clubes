namespace API_PI_Clubes.Application.Storage;
public sealed class ProcessedImageResult : IDisposable
{
    public string BaseName { get; }
    public IReadOnlyList<ProcessedImageVariant> Variants { get; }

    public ProcessedImageResult(string baseName, List<ProcessedImageVariant> variants)
    {
        BaseName = baseName;
        Variants = variants.AsReadOnly();
    }

    public void Dispose()
    {
        foreach (var v in Variants)
            v.Dispose();
    }
}