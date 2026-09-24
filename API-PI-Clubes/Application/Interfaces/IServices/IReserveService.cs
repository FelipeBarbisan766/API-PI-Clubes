using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface IReserveService
    {
        Task<IEnumerable<ResponseReserveDTO>> GetAll(CancellationToken cancellationToken);
        Task<ResponseReserveDTO> GetById(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<ResponseReserveDTO>> GetByClubId(Guid id, CancellationToken cancellationToken);
        Task<PagedResultDTO<ResponseReserveDetailDTO>> GetDetailedByClubId(Guid clubId, ReserveQueryDTO query, CancellationToken cancellationToken);
        Task<PagedResultDTO<ResponseReserveDetailToPlayerDTO>> GetDetailedByPlayerId(Guid playerId, ReserveQueryDTO query, CancellationToken cancellationToken);
        Task<ResponseIdDTO> Create(CreatReserveDTO dto, CancellationToken cancellationToken);
        Task ChangeStatus(Guid id, StatusEnum status, CancellationToken cancellationToken);
        Task<ResponseReserveDTO> Update(Guid id, UpdateReserveDTO dto, CancellationToken cancellationToken);
        Task Delete(Guid id, CancellationToken cancellationToken);
    }
}
