using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IMappers;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.Enums;
using Microsoft.EntityFrameworkCore;

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

            if (string.IsNullOrWhiteSpace(dto.UserName))
                throw new ArgumentException("UserName é obrigatório", nameof(dto.UserName));

            if (dto.UserId == Guid.Empty)
                throw new ArgumentException("UserId é obrigatório e deve ser válido", nameof(dto.UserId));

            var userExists = await _userService.GetById(dto.UserId);
            if (userExists == null)
                throw new InvalidOperationException("Usuário não encontrado");

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

                return new ResponseIdDTO { Id = entity.Id };
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Erro ao criar administrador", ex);
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