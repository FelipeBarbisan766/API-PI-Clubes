namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface IShareService
    {
        Task<string?> BuildClubSharePageAsync(Guid clubId, CancellationToken cancellationToken);
    }
}
