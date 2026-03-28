using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Model.Enums;
using Microsoft.AspNetCore.Mvc;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface IUserService
    {
        //Task<IEnumerable<ResponseUserDTO>> GetAll();
        Task<ResponseUserDTO> GetById(Guid id);
        Task Create(CreatUserDTO dto);
        Task<ResponseUserDTO> Update(Guid id, UpdateUserDTO dto);
        Task UpdateRole(Guid id, RoleEnum role);
        Task Delete(Guid id);
    }
}
