using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface IPlanService
    {
        Task<IEnumerable<ResponsePlanDTO>> GetAll();
        Task<ResponseIdDTO> Create(CreatPlanDTO dto);
        Task<ResponsePlanDTO> Update(Guid id, UpdatePlanDTO dto);
        Task<ResponsePlanDTO> SetActive(Guid id, bool Action);
    }
}
