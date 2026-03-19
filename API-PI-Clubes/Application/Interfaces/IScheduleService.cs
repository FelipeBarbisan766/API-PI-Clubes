using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces
{
    public interface IScheduleService
    {
        Task<IEnumerable<ResponseScheduleDTO>> GetAll();
        Task<ResponseScheduleDTO> GetById(Guid id);
        Task<ResponseScheduleDTO> Create(CreatScheduleDTO dto);
        Task<ResponseScheduleDTO> Update(Guid id, UpdateScheduleDTO dto);
        Task Delete(Guid id);
    }
}
