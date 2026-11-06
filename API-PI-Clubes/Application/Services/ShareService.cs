using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace API_PI_Clubes.Application.Services
{
    public class ShareService : IShareService
    {
        public const string CoverHttpClientName = "share-cover";

        private const int MaxCoverBytes = 5 * 1024 * 1024;
        private static readonly TimeSpan CoverTimeout = TimeSpan.FromSeconds(4);

        // Gerar imagem é custoso: no máximo 2 renders ao mesmo tempo.
        private static readonly SemaphoreSlim RenderGate = new(2);

        private readonly IClubRepository _clubRepository;
        private readonly IShareImageRenderer _renderer;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;
        private readonly ShareSettings _settings;
        private readonly ILogger<ShareService> _logger;

        public ShareService(
            IClubRepository clubRepository,
            IShareImageRenderer renderer,
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache,
            IOptions<ShareSettings> options,
            ILogger<ShareService> logger)
        {
            _clubRepository = clubRepository;
            _renderer = renderer;
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _settings = options.Value;
            _logger = logger;
        }

        public async Task<ShareImageResultDTO?> GetClubImageAsync(Guid clubId, CancellationToken cancellationToken)
        {
            var club = await _clubRepository.GetShareDataAsync(clubId, cancellationToken);
            if (club is null)
                return null;

            var version = ShareFormat.Version(club);
            var cacheKey = $"share-image:{club.Id}:{version}";

            if (_cache.TryGetValue(cacheKey, out byte[]? cached) && cached is not null)
                return new ShareImageResultDTO(cached, version);

            await RenderGate.WaitAsync(cancellationToken);
            try
            {
                // Outra requisição pode ter gerado enquanto esperávamos o semáforo.
                if (_cache.TryGetValue(cacheKey, out cached) && cached is not null)
                    return new ShareImageResultDTO(cached, version);

                var cover = await DownloadCoverAsync(club.CoverUrl, cancellationToken);
                var bytes = _renderer.RenderClub(club, cover);

                _cache.Set(cacheKey, bytes, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromHours(6)
                });

                return new ShareImageResultDTO(bytes, version);
            }
            finally
            {
                RenderGate.Release();
            }
        }

        /// <summary>
        /// Baixa a capa com host permitido, timeout curto e limite de tamanho.
        /// Qualquer falha devolve null: a imagem sai sem foto, nunca com erro 500.
        /// </summary>
        private async Task<byte[]?> DownloadCoverAsync(string? url, CancellationToken cancellationToken)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)
                || uri.Scheme != Uri.UriSchemeHttps
                || !_settings.AllowedImageHosts.Contains(uri.Host, StringComparer.OrdinalIgnoreCase))
            {
                return null;
            }

            try
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(CoverTimeout);

                var client = _httpClientFactory.CreateClient(CoverHttpClientName);
                using var response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cts.Token);

                if (!response.IsSuccessStatusCode || response.Content.Headers.ContentLength > MaxCoverBytes)
                    return null;

                await using var stream = await response.Content.ReadAsStreamAsync(cts.Token);
                using var ms = new MemoryStream();
                var buffer = new byte[81920];
                int read;

                while ((read = await stream.ReadAsync(buffer, cts.Token)) > 0)
                {
                    ms.Write(buffer, 0, read);
                    if (ms.Length > MaxCoverBytes)
                        return null;
                }

                return ms.ToArray();
            }
            catch (Exception ex) when (ex is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning(ex, "Falha ao baixar a capa para a imagem de compartilhamento ({Host})", uri.Host);
                return null;
            }
        }
    }
}