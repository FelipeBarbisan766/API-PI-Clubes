using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Model.Enums;
using Microsoft.AspNetCore.Mvc;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface IUserService
    {
        //Task<IEnumerable<ResponseUserDTO>> GetAll();
        Task<ResponseUserDTO> GetById(Guid id, CancellationToken cancellationToken);
        Task<ResponseUserDTO> Update(Guid id, UpdateUserDTO dto);
        Task UpdateAvatar(Guid id, UpdateAvatarDTO dto);
        Task UpdateRole(Guid id, RoleEnum role, CancellationToken cancellationToken);
        Task Delete(Guid id);
        Task<string> ProcessAvatarFromUrlAsync(string imageUrl);
    }
}
