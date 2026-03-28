using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Security;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<ResponseUserDTO> GetById(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new Exception("User not found");

            return new ResponseUserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
            };
        }

        public async Task Create(CreatUserDTO dto)
        {
            var userExists = await _userRepository.GetByEmailAsync(dto.Email);

            if (userExists != null)
                throw new Exception("User already exists");

            var entity = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = _passwordHasher.Hash(dto.Password),
                PhoneNumber = dto.PhoneNumber,
                Role = RoleEnum.None
            };

            await _userRepository.AddAsync(entity);
            await _userRepository.SaveChangesAsync();
        }

        public async Task<ResponseUserDTO> Update(Guid id, UpdateUserDTO dto)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new Exception("User not found");

            user.Name = dto.Name;
            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;
            user.UpdatedAt = DateTime.UtcNow;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return new ResponseUserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
            };
        }

        public async Task UpdateRole(Guid id, RoleEnum role)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new Exception("User not found");

            user.Role = (user.Role == RoleEnum.None) ? role : RoleEnum.Both;
            user.UpdatedAt = DateTime.UtcNow;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new Exception("User not found");

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }
    }
}
