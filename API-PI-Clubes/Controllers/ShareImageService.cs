using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using Microsoft.Extensions.Caching.Memory;

namespace API_PI_Clubes.Application.Services
{
    public class ShareImageService : IShareImageService
    {
        private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(6);

        private static readonly TimeSpan FallbackCacheTtl = TimeSpan.FromMinutes(2);

        private readonly IClubRepository _clubRepository;
        private readonly ICoverImageLoader _coverLoader;
        private readonly IShareImageRenderer _renderer;
        private readonly IMemoryCache _cache;

        public ShareImageService(
            IClubRepository clubRepository,
            ICoverImageLoader coverLoader,
            IShareImageRenderer renderer,
            IMemoryCache cache)
        {
            _clubRepository = clubRepository;
            _coverLoader = coverLoader;
            _renderer = renderer;
            _cache = cache;
        }

        public async Task<ShareImageResult?> GetClubImageAsync(Guid clubId, CancellationToken cancellationToken)
        {
            var club = await _clubRepository.GetShareDataAsync(clubId, cancellationToken);
            if (club is null)
                return null;

            var version = ShareFormat.Version(club);
            var key = $"share-image:{clubId}:{version}";

            if (_cache.TryGetValue(key, out byte[]? cached) && cached is not null)
                return new ShareImageResult(cached, version);

            var cover = await _coverLoader.LoadAsync(club.CoverUrl, cancellationToken);
            var bytes = _renderer.RenderClub(club, cover);

            var coverFailed = cover is null && !string.IsNullOrWhiteSpace(club.CoverUrl);
            _cache.Set(key, bytes, coverFailed ? FallbackCacheTtl : CacheTtl);

            return new ShareImageResult(bytes, version);
        }
    }
}
