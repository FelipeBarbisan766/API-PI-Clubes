using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public sealed record ShareImageResult(byte[] Bytes, string Version);

    public interface IShareImageService
    {
        Task<ShareImageResult?> GetClubImageAsync(Guid clubId, CancellationToken cancellationToken);
    }

    public interface IShareImageRenderer
    {
        byte[] RenderClub(ShareClubDataDTO club, byte[]? coverBytes);
    }

    public interface ICoverImageLoader
    {
        Task<byte[]?> LoadAsync(string? url, CancellationToken cancellationToken);
    }
}
