using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Infrastructure.Security;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API_PI_Clubes.Application.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        public UserService(AppDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        //public async Task<IEnumerable<ResponseUserDTO>> GetAll()
        //{
        //    return await _context.Users
        //        .Where(c => c.IsActive)
        //        .Select(c => new ResponseUserDTO
        //        {
        //            Id = c.Id,
        //            Name = c.Name,
        //            Email = c.Email,
        //            PhoneNumber = c.PhoneNumber,
        //            Role = c.Role
        //        })
        //        .ToListAsync();
        //}
        public async Task<ResponseUserDTO> GetById(Guid id)
        {
            var data = await _context.Users
                .Where(c => c.Id == id && c.IsActive)
                .Select(c => new ResponseUserDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                })
                .FirstOrDefaultAsync();

            if (data == null)
                throw new Exception("User not found");  

            return data;
        }

        public async Task Create(CreatUserDTO dto)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Email == dto.Email);

            if (userExists)
                throw new Exception("User already exists");

            var entity = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = _passwordHasher.Hash(dto.Password),
                PhoneNumber = dto.PhoneNumber,
                Role = RoleEnum.None
            };

            _context.Users.Add(entity);
            await _context.SaveChangesAsync();

        }

        public async Task<ResponseUserDTO> Update(Guid id, UpdateUserDTO dto)
        {
            var data = await _context.Users.FirstOrDefaultAsync(c => c.Id == id);

            if (data == null)
                throw new Exception("User not found");

            data.Name = dto.Name;
            data.Email = dto.Email;
            data.PhoneNumber = dto.PhoneNumber;
            data.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ResponseUserDTO
            {
                Id          = data.Id,
                Name        = data.Name,
                Email       = data.Email,
                PhoneNumber = data.PhoneNumber,
            };
        }
        public async Task UpdateRole(Guid id, RoleEnum role)
        {
            var data = await _context.Users.FirstOrDefaultAsync(c => c.Id == id);

            if (data == null)
                throw new Exception("User not found");

            if (data.Role == RoleEnum.None)
                data.Role = role;
            else
                data.Role = RoleEnum.Both;

            data.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
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
