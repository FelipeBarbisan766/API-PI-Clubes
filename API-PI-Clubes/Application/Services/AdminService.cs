using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IMappers;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Security.Interfaces;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace API_PI_Clubes.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _repository;
        private readonly IAdminMapper _mapper;
        private readonly IUserService _userService;

        public AdminService(IAdminMapper mapper, IAdminRepository repository, IUserService userService)
        {
            _mapper = mapper;
            _repository = repository;
            _userService = userService;
        }

        public async Task<ResponseAdminDTO> GetById(Guid id)
        {
            ValidateId(id);

            var data = await _repository.GetByIdAsync(id);

            if (data == null)
                throw new InvalidOperationException("Admin not found");

            return _mapper.ToDTO(data);
        }
        public async Task<ResponseIdDTO> Create(CreatAdminDTO dto)
        {
            ValidateAdminDTO(dto);

            var user = await _userService.GetById(dto.UserId)
                       ?? throw new KeyNotFoundException("Usuário não encontrado");

            using var transaction = (IDbContextTransaction)await _repository.BeginTransactionAsync();
            try
            {
                var entity = new Admin 
                {
                    UserName = dto.UserName,
                    ContactNumber = dto.ContactNumber,
                    Description = dto.Description,
                    UserId = dto.UserId,
                    TypeAccess = TypeAccessEnum.write,
                    CreatedAt = DateTime.UtcNow
                };
                await _repository.AddAsync(entity);

                await _userService.UpdateRole(dto.UserId, RoleEnum.Admin);

                await _repository.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ResponseIdDTO { Id = entity.Id };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task<ResponseAdminDTO> Update(Guid id, UpdateAdminDTO dto)
        {
            ValidateId(id);
            ValidateUpdateAdminDTO(dto);

            var data = await _repository.GetByIdAsync(id);

            if (data == null)
                throw new InvalidOperationException("Admin not found");

            data.UserName = dto.UserName;
            data.ContactNumber = dto.ContactNumber;
            data.Description = dto.Description;
            data.UpdatedAt = DateTime.UtcNow;

            _repository.Update(data);
            await _repository.SaveChangesAsync();

            return _mapper.ToDTO(data);
        }

        public async Task Delete(Guid id)
        {
            ValidateId(id);

            var exists = await _repository.ExistsAsync(id);

            if (!exists)
                throw new InvalidOperationException("Admin not found");

            await _repository.DeleteAsync(id);
        }

        private static void ValidateId(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid ID", nameof(id));
        }

        private static void ValidateAdminDTO(CreatAdminDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
        }

        private static void ValidateUpdateAdminDTO(UpdateAdminDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
        }
    }
}