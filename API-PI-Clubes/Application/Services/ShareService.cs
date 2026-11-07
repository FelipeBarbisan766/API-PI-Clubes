using System.Net;
using System.Text.Json;
using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace API_PI_Clubes.Application.Services
{
    public class ShareService : IShareService
    {
        private const int MaxDescriptionLength = 100;

        private readonly IClubRepository _clubRepository;
        private readonly ShareSettings _settings;

        public ShareService(IClubRepository clubRepository, IOptions<ShareSettings> options)
        {
            _clubRepository = clubRepository;
            _settings = options.Value;
        }

        public async Task<string?> BuildClubSharePageAsync(Guid clubId, CancellationToken cancellationToken)
        {
            var club = await _clubRepository.GetShareDataAsync(clubId, cancellationToken);
            if (club is null)
                return null;

            var apiBase = _settings.ApiPublicBaseUrl.TrimEnd('/');
            var frontBase = _settings.FrontendBaseUrl.TrimEnd('/');

            var shareUrl = $"{apiBase}/api/share/club/{club.Id}";
            var imageUrl = $"{apiBase}/api/club/{club.Id}/share-image?v={ShareFormat.Version(club)}";
            var redirectUrl = $"{frontBase}/clubs/{club.Id}";

            var name = WebUtility.HtmlEncode(club.Name);
            var description = WebUtility.HtmlEncode(BuildDescription(club));
            var shareUrlAttr = WebUtility.HtmlEncode(shareUrl);
            var imageUrlAttr = WebUtility.HtmlEncode(imageUrl);
            var redirectUrlAttr = WebUtility.HtmlEncode(redirectUrl);

            var redirectUrlJs = JsonSerializer.Serialize(redirectUrl);

            return $"""
                    <!doctype html>
                    <html lang="pt-BR">
                    <head>
                      <meta charset="utf-8" />
                      <title>{name} | Clubera</title>

                      <meta property="og:type" content="website" />
                      <meta property="og:site_name" content="Clubera" />
                      <meta property="og:locale" content="pt_BR" />
                      <meta property="og:title" content="{name}" />
                      <meta property="og:description" content="{description}" />
                      <meta property="og:image" content="{imageUrlAttr}" />
                      <meta property="og:image:type" content="image/jpeg" />
                      <meta property="og:image:width" content="1200" />
                      <meta property="og:image:height" content="630" />
                      <meta property="og:url" content="{shareUrlAttr}" />

                      <meta name="twitter:card" content="summary_large_image" />

                      <meta http-equiv="refresh" content="0;url={redirectUrlAttr}" />
                      <link rel="canonical" href="{redirectUrlAttr}" />
                    </head>
                    <body>
                      <script>location.replace({redirectUrlJs});</script>
                      <p>Redirecionando… <a href="{redirectUrlAttr}">Clique aqui</a>.</p>
                    </body>
                    </html>
                    """;
        }

        private static string BuildDescription(ShareClubDataDTO club)
        {
            var parts = new List<string>();

            if (club.MinPrice is > 0)
                parts.Add($"A partir de R$ {ShareFormat.Price(club.MinPrice.Value)}/h");

            if (club.Sports.Count > 0)
                parts.Add(string.Join(", ", club.Sports.OrderBy(s => s)));

            var location = string.Join("/", new[] { club.City, club.State }
                .Where(s => !string.IsNullOrWhiteSpace(s)));
            if (location.Length > 0)
                parts.Add(location);

            var text = parts.Count > 0
                ? string.Join(" • ", parts)
                : "Reserve sua quadra na Clubera";

            return text.Length <= MaxDescriptionLength
                ? text
                : text[..(MaxDescriptionLength - 1)].TrimEnd() + "…";
        }
    }
}
