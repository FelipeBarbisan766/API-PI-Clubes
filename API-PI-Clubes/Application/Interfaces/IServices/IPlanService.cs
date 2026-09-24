using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface IPlanService
    {
        Task<IEnumerable<PlanResponseDto>> GetAllActiveAsync(CancellationToken cancellationToken);
        Task<PlanResponseDto> CreateAsync(CreatePlanDto dto, CancellationToken cancellationToken);
        Task<PlanResponseDto> UpdateAsync(Guid id, UpdatePlanDto dto, CancellationToken cancellationToken);
        Task SetActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken);

    }
}
