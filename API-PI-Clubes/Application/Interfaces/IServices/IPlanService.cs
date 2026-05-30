using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface IPlanService
    {
        Task<IEnumerable<PlanResponseDto>> GetAllActiveAsync();
        Task<PlanResponseDto> CreateAsync(CreatePlanDto dto);
        Task<PlanResponseDto> UpdateAsync(Guid id, UpdatePlanDto dto);
        Task SetActiveAsync(Guid id, bool isActive);

    }
}
