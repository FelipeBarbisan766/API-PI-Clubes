using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Application.Services;
using SkiaSharp;

namespace API_PI_Clubes.Infrastructure.Rendering
{
    
    public sealed class ShareImageRenderer : IShareImageRenderer
    {
        private const int W = 1200, H = 630;
        private const int MaxBytes = 300 * 1024;

        private readonly SKTypeface _regular = LoadTypeface("Fonts/Roboto-Regular.ttf");
        private readonly SKTypeface _bold = LoadTypeface("Fonts/Roboto-Bold.ttf");
        private readonly SKTypeface _display = LoadTypeface("Fonts/SpaceGrotesk-Bold.ttf");
        private readonly SKBitmap? _logo = LoadLogo("Assets/logo.png");

        public byte[] RenderClub(ShareClubDataDTO club, byte[]? coverBytes)
        {
            using var surface = SKSurface.Create(new SKImageInfo(W, H));
            var canvas = surface.Canvas;
            canvas.Clear(SKColor.Parse("#3a0717"));

            DrawCover(canvas, coverBytes);
            DrawGradient(canvas);
            DrawLogo(canvas);

            if (club.MinPrice is > 0)
                DrawPriceBadge(canvas, club.MinPrice.Value);

            DrawTexts(canvas, club);

            using var image = surface.Snapshot();

            foreach (var quality in new[] { 80, 70, 60, 50 })
            {
                using var data = image.Encode(SKEncodedImageFormat.Jpeg, quality);
                if (data.Size <= MaxBytes || quality == 50)
                    return data.ToArray();
            }

            throw new InvalidOperationException("Unreachable");
        }

        private static void DrawCover(SKCanvas canvas, byte[]? bytes)
        {
            if (bytes is null) return;

            using var bmp = SKBitmap.Decode(bytes);
            if (bmp is null) return;

            var scale = Math.Max((float)W / bmp.Width, (float)H / bmp.Height);
            var dw = bmp.Width * scale;
            var dh = bmp.Height * scale;
            var dest = new SKRect((W - dw) / 2, (H - dh) / 2, (W + dw) / 2, (H + dh) / 2);

            using var img = SKImage.FromBitmap(bmp);
            canvas.DrawImage(img, dest, new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear));
        }

        private static void DrawGradient(SKCanvas canvas)
        {
            using var paint = new SKPaint
            {
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, H * 0.30f),
                    new SKPoint(0, H),
                    [SKColors.Transparent, new SKColor(0, 0, 0, 215)],
                    SKShaderTileMode.Clamp)
            };
            canvas.DrawRect(0, 0, W, H, paint);
        }

        private void DrawLogo(SKCanvas canvas)
        {
            if (_logo is null) return;

            const float targetH = 56f;
            var targetW = _logo.Width * (targetH / _logo.Height);

            using var img = SKImage.FromBitmap(_logo);
            canvas.DrawImage(
                img,
                new SKRect(48, 40, 48 + targetW, 40 + targetH),
                new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear));
        }

        private void DrawPriceBadge(SKCanvas canvas, decimal price)
        {
            var value = ShareFormat.Price(price);
            if (value.EndsWith(",00")) value = value[..^3]; // "80,00" -> "80"
            var priceText = $"R$ {value}/h";
            const string label = "A PARTIR DE";

            using var small = new SKFont(_bold, 22);
            using var big = new SKFont(_display, 56);

            var width = Math.Max(small.MeasureText(label), big.MeasureText(priceText)) + 56;
            const float height = 112, right = W - 48, top = 40;
            var rect = new SKRect(right - width, top, right, top + height);

            using var bg = new SKPaint { Color = SKColor.Parse("#870027"), IsAntialias = true };
            canvas.DrawRoundRect(rect, 28, 28, bg);

            using var white = new SKPaint { Color = SKColors.White, IsAntialias = true };
            canvas.DrawText(label, rect.Left + 28, top + 34, SKTextAlign.Left, small, white);
canvas.DrawText(priceText, rect.Left + 28, top + 92, SKTextAlign.Left, big, white);

        }

        private void DrawTexts(SKCanvas canvas, ShareClubDataDTO club)
        {
            using var white = new SKPaint { Color = SKColors.White, IsAntialias = true };
            using var soft = new SKPaint { Color = new SKColor(255, 255, 255, 225), IsAntialias = true };
            using var nameFont = new SKFont(_display, 68);
            using var subFont = new SKFont(_regular, 34);

            var location = string.Join("/", new[] { club.City, club.State }
                .Where(s => !string.IsNullOrWhiteSpace(s)));

            var sub = string.Join("  •  ", new[] { location, string.Join(" • ", club.Sports.OrderBy(s => s)) }
                .Where(s => !string.IsNullOrWhiteSpace(s)));

            var lines = Wrap(club.Name, nameFont, W - 96, 2);

            const float lineHeight = 78;
            const float subY = H - 48f;
            var firstNameY = subY - 56 - (lines.Count - 1) * lineHeight;

            for (var i = 0; i < lines.Count; i++)
                canvas.DrawText(lines[i], 48, firstNameY + i * lineHeight, SKTextAlign.Left, nameFont, white);

            if (sub.Length > 0)
                canvas.DrawText(sub, 48, subY, SKTextAlign.Left, subFont, soft);
        }

        private static List<string> Wrap(string text, SKFont font, float maxWidth, int maxLines)
        {
            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var lines = new List<string>();
            var current = string.Empty;

            for (var i = 0; i < words.Length; i++)
            {
                var candidate = current.Length == 0 ? words[i] : $"{current} {words[i]}";
                if (font.MeasureText(candidate) <= maxWidth)
                {
                    current = candidate;
                    continue;
                }

                if (current.Length > 0)
                    lines.Add(current);

                if (lines.Count == maxLines - 1)
                {
                    current = string.Join(' ', words.Skip(i));
                    break;
                }

                current = words[i];
            }

            if (current.Length > 0)
                lines.Add(current);

            if (lines.Count == 0)
                lines.Add(string.Empty);

            var last = lines.Count - 1;
            lines[last] = Ellipsize(lines[last], font, maxWidth);
            return lines;
        }

        private static string Ellipsize(string text, SKFont font, float maxWidth)
        {
            if (font.MeasureText(text) <= maxWidth) return text;

            while (text.Length > 1 && font.MeasureText(text + "…") > maxWidth)
                text = text[..^1];

            return text.TrimEnd() + "…";
        }

        private static SKTypeface LoadTypeface(string relativePath)
        {
            var path = Path.Combine(AppContext.BaseDirectory, relativePath);
            return File.Exists(path) ? SKTypeface.FromFile(path) : SKTypeface.Default;
        }

        private static SKBitmap? LoadLogo(string relativePath)
        {
            var path = Path.Combine(AppContext.BaseDirectory, relativePath);
            return File.Exists(path) ? SKBitmap.Decode(path) : null;
        }
    }
}