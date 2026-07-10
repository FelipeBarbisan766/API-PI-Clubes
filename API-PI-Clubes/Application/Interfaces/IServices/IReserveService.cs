using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface IReserveService
    {
        Task<IEnumerable<ResponseReserveDTO>> GetAll();
        Task<ResponseReserveDTO> GetById(Guid id);
        Task<IEnumerable<ResponseReserveDTO>> GetByClubId(Guid id);
        Task<IEnumerable<ResponseReserveDetailDTO>> GetDetailedByClubId(Guid clubId);
        Task<IEnumerable<ResponseReserveDetailToPlayerDTO>> GetDetailedByPlayerId(Guid playerId);
        Task<ResponseIdDTO> Create(CreatReserveDTO dto);
        Task ChangeStatus(Guid id, StatusEnum status);
        Task<ResponseReserveDTO> Update(Guid id, UpdateReserveDTO dto);
        Task Delete(Guid id);
    }
}
