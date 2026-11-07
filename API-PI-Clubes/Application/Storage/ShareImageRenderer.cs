using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Application.Services;
using SkiaSharp;

namespace API_PI_Clubes.Infrastructure.Services
{
    public sealed class ShareImageRenderer : IShareImageRenderer, IDisposable
    {
        private const int W = 1200;
        private const int H = 630;
        private const float Margin = 64f;        
        private const float TopPadding = 44f;    
        private const float BottomPadding = 52f; 
        private const int MaxBytes = 290_000;    

        private static readonly SKColor Wine = SKColor.Parse("#870027");     
        private static readonly SKColor WineDark = SKColor.Parse("#3a0717"); 

        private readonly SKTypeface _regular;
        private readonly SKTypeface _bold;
        private readonly SKTypeface _display; 
        private readonly SKImage? _logo;      

        public ShareImageRenderer()
        {
            
            var assets = Path.Combine(AppContext.BaseDirectory, "Assets");

            _regular = LoadRequiredTypeface(Path.Combine(assets, "Fonts", "Roboto-Regular.ttf"));
            _bold = LoadRequiredTypeface(Path.Combine(assets, "Fonts", "Roboto-Bold.ttf"));
            _display = SKTypeface.FromFile(Path.Combine(assets, "Fonts", "SpaceGrotesk-Bold.ttf")) ?? _bold;

            var logoPath = Path.Combine(assets, "Share", "logo.png");
            _logo = File.Exists(logoPath) ? SKImage.FromEncodedData(logoPath) : null;
        }

        public byte[] RenderClub(ShareClubDataDTO club, byte[]? coverBytes)
        {
            using var surface = SKSurface.Create(new SKImageInfo(W, H));
            var canvas = surface.Canvas;

            DrawBackground(canvas, coverBytes);
            DrawOverlays(canvas);
            DrawLogo(canvas);
            DrawPriceBadge(canvas, club.MinPrice);
            DrawTexts(canvas, club);

            using var snapshot = surface.Snapshot(); 
            return EncodeJpeg(snapshot);
        }

        private static void DrawBackground(SKCanvas canvas, byte[]? coverBytes)
        {
            canvas.Clear(WineDark);

            using var cover = coverBytes is null ? null : SKImage.FromEncodedData(coverBytes);

            if (cover is null)
            {
                using var shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0), new SKPoint(W, H),
                    new[] { Wine, WineDark }, new[] { 0f, 1f }, SKShaderTileMode.Clamp);
                using var paint = new SKPaint { Shader = shader };
                canvas.DrawRect(0, 0, W, H, paint);
                return;
            }

            var scale = Math.Max((float)W / cover.Width, (float)H / cover.Height);
            var srcW = W / scale;
            var srcH = H / scale;
            var src = new SKRect(
                (cover.Width - srcW) / 2f, (cover.Height - srcH) / 2f,
                (cover.Width + srcW) / 2f, (cover.Height + srcH) / 2f);

            DrawImage(canvas, cover, src, new SKRect(0, 0, W, H));
        }

        private static void DrawOverlays(SKCanvas canvas)
        {
            FillVerticalGradient(canvas, H * 0.28f, H, SKColors.Transparent, new SKColor(0, 0, 0, 225));

            FillVerticalGradient(canvas, 0, 170, new SKColor(0, 0, 0, 120), SKColors.Transparent);
        }

        private static void FillVerticalGradient(SKCanvas canvas, float top, float bottom, SKColor from, SKColor to)
        {
            using var shader = SKShader.CreateLinearGradient(
                new SKPoint(0, top), new SKPoint(0, bottom),
                new[] { from, to }, new[] { 0f, 1f }, SKShaderTileMode.Clamp);
            using var paint = new SKPaint { Shader = shader };
            canvas.DrawRect(0, top, W, bottom - top, paint); // x, y, largura, altura
        }

        private void DrawLogo(SKCanvas canvas)
        {
            if (_logo is not null)
            {
                const float logoHeight = 56f; 
                var logoWidth = _logo.Width * (logoHeight / _logo.Height);
                DrawImage(canvas, _logo,
                    new SKRect(0, 0, _logo.Width, _logo.Height),
                    SKRect.Create(Margin, TopPadding, logoWidth, logoHeight));
                return;
            }

            using var font = new SKFont(_display, 42);
            using var paint = new SKPaint { IsAntialias = true, Color = SKColors.White };
            DrawShadowedText(canvas, "CLUBERA", Margin, TopPadding - font.Metrics.Ascent, font, paint);
        }

        private void DrawPriceBadge(SKCanvas canvas, decimal? minPrice)
        {
            if (minPrice is null or <= 0)
                return;

            using var smallFont = new SKFont(_bold, 22);
            using var bigFont = new SKFont(_display, 58);
            const string small = "A PARTIR DE";
            var big = $"R$ {ShareFormat.PriceShort(minPrice.Value)}/h";

            const float padX = 30f, padY = 18f, gap = 4f;

            var smallW = smallFont.MeasureText(small);
            var bigW = bigFont.MeasureText(big);
            var badgeW = Math.Max(smallW, bigW) + padX * 2;
            var badgeH = padY + smallFont.Spacing + gap + bigFont.Spacing + padY;

            var rect = SKRect.Create(W - Margin - badgeW, TopPadding - 4, badgeW, badgeH);

            using var fill = new SKPaint { IsAntialias = true, Color = Wine, Style = SKPaintStyle.Fill };
            canvas.DrawRoundRect(rect, 24, 24, fill);
            using var border = new SKPaint
            {
                IsAntialias = true,
                Color = new SKColor(255, 255, 255, 70),
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2
            };
            canvas.DrawRoundRect(rect, 24, 24, border);

            using var text = new SKPaint { IsAntialias = true, Color = SKColors.White };
            var y1 = rect.Top + padY - smallFont.Metrics.Ascent;
            canvas.DrawText(small, rect.Left + (badgeW - smallW) / 2f, y1, SKTextAlign.Left, smallFont, text);

            var y2 = rect.Top + padY + smallFont.Spacing + gap - bigFont.Metrics.Ascent;
            canvas.DrawText(big, rect.Left + (badgeW - bigW) / 2f, y2, SKTextAlign.Left, bigFont, text);
        }

        private void DrawTexts(SKCanvas canvas, ShareClubDataDTO club)
        {
            var maxWidth = W - Margin * 2;

            using var nameFont = new SKFont(_bold, 68);
            using var locationFont = new SKFont(_regular, 34);
            using var sportsFont = new SKFont(_regular, 30);
            using var paint = new SKPaint { IsAntialias = true };

            var location = string.Join("/", new[] { club.City, club.State }
                .Where(s => !string.IsNullOrWhiteSpace(s)));
            var sports = string.Join("  •  ", club.Sports.OrderBy(s => s));

            var cursorY = H - BottomPadding;

            if (sports.Length > 0)
            {
                paint.Color = new SKColor(255, 255, 255, 215);
                DrawShadowedText(canvas, Ellipsize(sports, sportsFont, maxWidth), Margin, cursorY, sportsFont, paint);
                cursorY -= locationFont.Spacing;
            }

            if (location.Length > 0)
            {
                paint.Color = SKColors.White;
                DrawShadowedText(canvas, Ellipsize(location, locationFont, maxWidth), Margin, cursorY, locationFont, paint);
                cursorY -= nameFont.Spacing * 0.95f; 
            }

            var nameLines = WrapText(club.Name, nameFont, maxWidth, maxLines: 2);
            paint.Color = SKColors.White;
            for (var i = nameLines.Count - 1; i >= 0; i--)
            {
                DrawShadowedText(canvas, nameLines[i], Margin, cursorY, nameFont, paint);
                cursorY -= nameFont.Spacing * 0.95f;
            }
        }

        private static void DrawShadowedText(SKCanvas canvas, string text, float x, float y, SKFont font, SKPaint paint)
        {
            var original = paint.Color;
            paint.Color = new SKColor(0, 0, 0, 140);
            canvas.DrawText(text, x + 2, y + 2, SKTextAlign.Left, font, paint);
            paint.Color = original;
            canvas.DrawText(text, x, y, SKTextAlign.Left, font, paint);
        }

        private static void DrawImage(SKCanvas canvas, SKImage image, SKRect src, SKRect dest)
        {
            var sampling = new SKSamplingOptions(SKCubicResampler.Mitchell);
            using var paint = new SKPaint { IsAntialias = true };
            canvas.DrawImage(image, src, dest, sampling, paint);
        }

        private static List<string> WrapText(string text, SKFont font, float maxWidth, int maxLines)
        {
            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var lines = new List<string>();
            var current = string.Empty;

            foreach (var word in words)
            {
                var candidate = current.Length == 0 ? word : current + " " + word;

                if (font.MeasureText(candidate) <= maxWidth)
                {
                    current = candidate;
                    continue;
                }

                if (current.Length > 0)
                    lines.Add(current);
                current = word; 
            }

            if (current.Length > 0)
                lines.Add(current);

            if (lines.Count == 0)
                lines.Add(string.Empty);

            if (lines.Count > maxLines)
            {
                var overflow = string.Join(" ", lines.Skip(maxLines - 1));
                lines = lines.Take(maxLines - 1).Append(overflow).ToList();
            }

            return lines.Select(l => Ellipsize(l, font, maxWidth)).ToList();
        }

        private static string Ellipsize(string text, SKFont font, float maxWidth)
        {
            if (font.MeasureText(text) <= maxWidth)
                return text;

            const string dots = "…";
            var end = text.Length;
            while (end > 0 && font.MeasureText(text[..end].TrimEnd() + dots) > maxWidth)
                end--;

            return end == 0 ? dots : text[..end].TrimEnd() + dots;
        }

        private static byte[] EncodeJpeg(SKImage image)
        {
            var result = Array.Empty<byte>();

            for (var quality = 80; quality >= 45; quality -= 10)
            {
                using var data = image.Encode(SKEncodedImageFormat.Jpeg, quality);
                result = data.ToArray();
                if (result.Length <= MaxBytes)
                    break;
            }

            return result;
        }

        private static SKTypeface LoadRequiredTypeface(string path) =>
            SKTypeface.FromFile(path)
            ?? throw new InvalidOperationException(
                $"Fonte não encontrada ou inválida: '{path}'. Confira se o .ttf está em Assets/Fonts " +
                "e se o .csproj copia a pasta Assets para o output.");

        public void Dispose()
        {
            _regular.Dispose();
            _bold.Dispose();
            if (!ReferenceEquals(_display, _bold))
                _display.Dispose();
            _logo?.Dispose();
        }
    }
}