namespace API_PI_Clubes.Application.DTOs;

public class FlagDTO
{
    public record FlagDto(Guid Id, string Name, string? Description);
    public record FlagTypeCountDto(Guid FlagId, string Name, int Count);
    public record CreatePlayerFlagRequest(Guid FlagId, Guid? ReserveId, string? Notes);
    public record PlayerFlagDto(
        Guid Id,
        Guid FlagId,
        string FlagName,
        Guid? ReserveId,
        string? Notes,
        DateTime CreatedAt);
}