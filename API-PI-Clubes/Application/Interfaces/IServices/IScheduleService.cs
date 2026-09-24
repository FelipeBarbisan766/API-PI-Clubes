using API_PI_Clubes.Application.DTOs;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface IScheduleService
    {
        Task<IEnumerable<ResponseScheduleDTO>> GetAll(CancellationToken cancellationToken);
        Task<ResponseScheduleDTO> GetById(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<ResponseScheduleDTO>> GetByCourtId(Guid courtId, CancellationToken cancellationToken);
        Task<IEnumerable<ResponseScheduleAvailabilityDTO>> GetAvailabilityByCourtAndDate(Guid courtId, DateOnly date, CancellationToken cancellationToken);
        Task<ResponseIdDTO> Create(CreatScheduleDTO dto, CancellationToken cancellationToken);
        Task<ResponseBulkScheduleDTO> CreateBulk(CreateBulkScheduleDTO dto, CancellationToken cancellationToken);
        Task<ResponseScheduleDTO> Update(Guid userId, Guid id, UpdateScheduleDTO dto, CancellationToken cancellationToken);
        Task Delete(Guid userId, Guid id, CancellationToken cancellationToken);
    }
}
