using SkiaSharp;
using API_PI_Clubes.Application.Storage;
using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Application.Storage;

public class ImageProcessingService : IImageProcessingService
{
    private record VariantConfig(
        ImageVariantType Type,
        int              MaxWidth,
        int              MaxHeight,
        string           Suffix,
        bool             CenterCrop   // true = thumb quadrado, false = max-fit sem upscale
    );

    private static readonly IReadOnlyList<VariantConfig> Variants = new[]
    {
        new VariantConfig(ImageVariantType.Thumb,  200,  200,  "thumb",  CenterCrop: true),
        new VariantConfig(ImageVariantType.Medium, 800,  600,  "medium", CenterCrop: false),
        new VariantConfig(ImageVariantType.Full, 1920, 1080,  "full",   CenterCrop: false),
    };

    private const int WebpQuality = 82;

    public async Task<ProcessedImageResult> ProcessAsync(Stream inputStream)
    {
        var baseName  = Guid.NewGuid().ToString();
        var processed = new List<ProcessedImageVariant>();

        // Carrega o stream em memória uma vez para o Skia decodificar
        using var buffer = new MemoryStream();
        await inputStream.CopyToAsync(buffer);
        var imageBytes = buffer.ToArray();

        using var original = SKBitmap.Decode(imageBytes)
            ?? throw new InvalidOperationException("Não foi possível decodificar a imagem.");

        foreach (var cfg in Variants)
        {
            using var resized = cfg.CenterCrop
                ? CenterCropAndResize(original, cfg.MaxWidth, cfg.MaxHeight)
                : ResizeMax(original, cfg.MaxWidth, cfg.MaxHeight);

            var outputStream = new MemoryStream();

            using var skImage  = SKImage.FromBitmap(resized);
            using var encoded  = skImage.Encode(SKEncodedImageFormat.Webp, WebpQuality);
            encoded.SaveTo(outputStream);
            outputStream.Seek(0, SeekOrigin.Begin);

            processed.Add(new ProcessedImageVariant(
                stream:   outputStream,
                fileName: $"{baseName}_{cfg.Suffix}.webp",
                variant:  cfg.Type
            ));
        }

        return new ProcessedImageResult(baseName, processed);
    }

    // ─── Resize proporcional, sem upscale ────────────────────────────────────
    private static SKBitmap ResizeMax(SKBitmap source, int maxWidth, int maxHeight)
    {
        // Imagem já cabe nos limites → devolve cópia sem redimensionar
        if (source.Width <= maxWidth && source.Height <= maxHeight)
            return source.Copy();

        var ratio     = Math.Min((double)maxWidth / source.Width, (double)maxHeight / source.Height);
        var newWidth  = (int)(source.Width  * ratio);
        var newHeight = (int)(source.Height * ratio);

        return source.Resize(new SKImageInfo(newWidth, newHeight), new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear))
            ?? throw new InvalidOperationException("Falha ao redimensionar imagem (ResizeMax).");
    }

    // ─── Crop centralizado + resize para dimensão exata (thumb) ─────────────
    private static SKBitmap CenterCropAndResize(SKBitmap source, int targetWidth, int targetHeight)
    {
        var sourceRatio = (double)source.Width  / source.Height;
        var targetRatio = (double)targetWidth   / targetHeight;

        int cropW, cropH, cropX, cropY;

        if (sourceRatio > targetRatio)
        {
            // imagem mais larga que o alvo → corta as laterais
            cropH = source.Height;
            cropW = (int)(cropH * targetRatio);
            cropX = (source.Width - cropW) / 2;
            cropY = 0;
        }
        else
        {
            // imagem mais alta que o alvo → corta topo/baixo
            cropW = source.Width;
            cropH = (int)(cropW / targetRatio);
            cropX = 0;
            cropY = (source.Height - cropH) / 2;
        }

        // 1. Recorte centralizado
        var cropped = new SKBitmap(cropW, cropH);
        try
        {
            using var canvas = new SKCanvas(cropped);
            canvas.DrawBitmap(
                source,
                SKRect.Create(cropX, cropY, cropW, cropH),
                SKRect.Create(0,     0,     cropW, cropH)
            );

            // 2. Resize para as dimensões exatas do thumb
            return cropped.Resize(new SKImageInfo(targetWidth, targetHeight), new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear))
                ?? throw new InvalidOperationException("Falha ao redimensionar imagem (CenterCrop).");
        }
        finally
        {
            cropped.Dispose();
        }
    }
}