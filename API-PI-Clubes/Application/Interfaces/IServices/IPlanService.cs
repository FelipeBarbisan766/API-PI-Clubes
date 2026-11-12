using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface IPlanService
    {
        Task<IEnumerable<PlanResponseDto>> GetAllActiveAsync(CancellationToken cancellationToken);
        Task<IEnumerable<PlanResponseDto>> GetMyCustomAsync(Guid userId, CancellationToken ct);
        Task<PlanResponseDto> CreateAsync(Guid userId,CreatePlanDto dto, CancellationToken cancellationToken);
        Task<PlanResponseDto> CreateCustomAsync(Guid userId, CustomPlanRequestDTO req, CancellationToken ct);
        Task<PlanResponseDto> UpdateAsync(Guid userId,Guid id, UpdatePlanDto dto, CancellationToken cancellationToken);
        Task SetActiveAsync(Guid userId, Guid id, bool isActive, CancellationToken cancellationToken);
    }
}