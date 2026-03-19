using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_PI_Clubes.Application.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ResponseUserDTO>> GetAll()
        {
            return await _context.Users
                .Where(c => c.IsActive)
                .Select(c => new ResponseUserDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Email = c.Email,
                    Password = c.Password,
                    PhoneNumber = c.PhoneNumber,
                    Role = c.Role
                })
                .ToListAsync();
        }
        public async Task<ResponseUserDTO> GetById(Guid id)
        {
            var data = await _context.Users
                .Where(c => c.Id == id && c.IsActive)
                .Select(c => new ResponseUserDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Email = c.Email,
                    Password = c.Password,
                    PhoneNumber = c.PhoneNumber,
                    Role = c.Role
                })
                .FirstOrDefaultAsync();

            if (data == null)
                throw new Exception("User not found");

            return data;
        }

        public async Task<ResponseUserDTO> Create(CreatUserDTO dto)
        {
            var entity = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = dto.Password,
                PhoneNumber= dto.PhoneNumber,
                Role = dto.Role
            };

            _context.Users.Add(entity);
            await _context.SaveChangesAsync();

            return new ResponseUserDTO
            {
                Id          = entity.Id,
                Name        = entity.Name,
                Email       = entity.Email,
                Password    = entity.Password,
                PhoneNumber = entity.PhoneNumber,
                Role        = entity.Role
            };
        }

        public async Task<ResponseUserDTO> Update(Guid id, UpdateUserDTO dto)
        {
            var data = await _context.Users.FirstOrDefaultAsync(c => c.Id == id);

            if (data == null)
                throw new Exception("User not found");

            data.Name = dto.Name;
            data.Email = dto.Email;
            data.Password = dto.Password;
            data.PhoneNumber = dto.PhoneNumber;
            data.Role = dto.Role;
            data.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ResponseUserDTO
            {
                Id          = data.Id,
                Name        = data.Name,
                Email       = data.Email,
                Password    = data.Password,
                PhoneNumber = data.PhoneNumber,
                Role        = data.Role
            };
        }

        public async Task Delete(Guid id)
        {
            var data = await _context.Users.FirstOrDefaultAsync(c => c.Id == id);

            if (data == null)
                throw new Exception("User not found");

            data.IsActive = false;
            data.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
