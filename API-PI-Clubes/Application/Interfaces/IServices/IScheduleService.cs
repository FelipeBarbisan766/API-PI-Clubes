using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface IScheduleService
    {
        Task<IEnumerable<ResponseScheduleDTO>> GetAll();
        Task<ResponseScheduleDTO> GetById(Guid id);
        Task<IEnumerable<ResponseScheduleDTO>> GetByCourtId(Guid courtId);
        Task<IEnumerable<ResponseScheduleAvailabilityDTO>> GetAvailabilityByCourtAndDate(Guid courtId, DateOnly date);
        Task<ResponseIdDTO> Create(CreatScheduleDTO dto);
        Task<ResponseBulkScheduleDTO> CreateBulk(CreateBulkScheduleDTO dto);
        Task<ResponseScheduleDTO> Update(Guid id, UpdateScheduleDTO dto);
        Task Delete(Guid id);
    }
}
