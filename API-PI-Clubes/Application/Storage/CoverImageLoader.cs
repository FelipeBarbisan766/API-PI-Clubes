using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace API_PI_Clubes.Infrastructure.Services
{
    public sealed class CoverImageLoader : ICoverImageLoader
    {
        public const string HttpClientName = "share-cover";
        private const int MaxBytes = 5 * 1024 * 1024; 

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _env;
        private readonly ShareSettings _settings;
        private readonly ILogger<CoverImageLoader> _logger;

        public CoverImageLoader(
            IHttpClientFactory httpClientFactory,
            IWebHostEnvironment env,
            IOptions<ShareSettings> options,
            ILogger<CoverImageLoader> logger)
        {
            _httpClientFactory = httpClientFactory;
            _env = env;
            _settings = options.Value;
            _logger = logger;
        }

        public async Task<byte[]?> LoadAsync(string? url, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            try
            {
                return url.StartsWith('/')
                    ? await LoadFromWebRootAsync(url, cancellationToken)
                    : await DownloadAsync(url, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw; 
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha ao carregar a capa para a imagem de compartilhamento: {Url}", url);
                return null;
            }
        }

        private async Task<byte[]?> DownloadAsync(string url, CancellationToken cancellationToken)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp))
                return null;

            if (IsOwnApiHost(uri))
                return await LoadFromWebRootAsync(Uri.UnescapeDataString(uri.AbsolutePath), cancellationToken);

            if (!_settings.AllowedImageHosts.Contains(uri.Host, StringComparer.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Host de capa fora da allowlist (Share:AllowedImageHosts): {Host}", uri.Host);
                return null;
            }

            var client = _httpClientFactory.CreateClient(HttpClientName);
            using var response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
                return null;

            if (response.Content.Headers.ContentLength > MaxBytes)
                return null;

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var buffer = new MemoryStream();
            var chunk = new byte[81920];
            int read;

            while ((read = await stream.ReadAsync(chunk, cancellationToken)) > 0)
            {
                if (buffer.Length + read > MaxBytes)
                    return null;
                buffer.Write(chunk, 0, read);
            }

            return buffer.ToArray();
        }

        private bool IsOwnApiHost(Uri uri)
        {
            if (uri.IsLoopback)
                return true;

            return Uri.TryCreate(_settings.ApiPublicBaseUrl, UriKind.Absolute, out var own)
                   && string.Equals(own.Host, uri.Host, StringComparison.OrdinalIgnoreCase)
                   && own.Port == uri.Port;
        }

        private async Task<byte[]?> LoadFromWebRootAsync(string relativeUrl, CancellationToken cancellationToken)
        {
            var root = Path.GetFullPath(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"));
            var relative = relativeUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var full = Path.GetFullPath(Path.Combine(root, relative));

            if (!full.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.Ordinal))
                return null;

            var info = new FileInfo(full);
            if (!info.Exists || info.Length > MaxBytes)
                return null;

            return await File.ReadAllBytesAsync(full, cancellationToken);
        }
    }
}