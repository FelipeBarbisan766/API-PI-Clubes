using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface IShareImageRenderer
    {
        byte[] RenderClub(ShareClubDataDTO club, byte[]? coverBytes);
    }
}