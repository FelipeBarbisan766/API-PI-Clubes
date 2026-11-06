using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Services
{
    public static class ShareFormat
    {
        public static string Version(ShareClubDataDTO club)
        {
            var raw = string.Join('|', new[]
            {
                club.Name,
                club.City,
                club.State,
                club.MinPrice?.ToString("0.00", CultureInfo.InvariantCulture) ?? string.Empty,
                club.CoverImageId?.ToString() ?? string.Empty,
                string.Join(',', club.Sports.OrderBy(s => s, StringComparer.Ordinal))
            });

            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
            return Convert.ToHexString(bytes, 0, 8).ToLowerInvariant();
        }
        
        public static string Price(decimal value) =>
            value.ToString("0.00", CultureInfo.InvariantCulture).Replace('.', ',');
    }
}
