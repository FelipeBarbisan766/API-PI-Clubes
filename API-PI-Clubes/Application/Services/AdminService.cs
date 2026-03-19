using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _context;
        public AdminService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ResponseAdminDTO>> GetAll()
        {
            return await _context.Admins
                .Where(c => c.IsActive)
                .Select(c => new ResponseAdminDTO
                {
                    Id = c.Id,
                    UserName = c.UserName,
                    ContactNumber = c.ContactNumber,
                    Description = c.Description,
                    TypeAccess = c.TypeAccess,
                    UserId = c.UserId
                })
                .ToListAsync();
        }
        public async Task<ResponseAdminDTO> GetById(Guid id)
        {
            var data = await _context.Admins
                .Where(c => c.Id == id && c.IsActive)
                .Select(c => new ResponseAdminDTO
                {
                    Id = c.Id,
                    UserName = c.UserName,
                    ContactNumber = c.ContactNumber,
                    Description = c.Description,
                    TypeAccess = c.TypeAccess,
                    UserId = c.UserId
                })
                .FirstOrDefaultAsync();

            if (data == null)
                throw new Exception("Admin not found");

            return data;
        }

        public async Task<ResponseAdminDTO> Create(CreatAdminDTO dto)
        {
            var entity = new Admin
            {
                UserName = dto.UserName,
                ContactNumber = dto.ContactNumber,
                Description = dto.Description,
                TypeAccess = dto.TypeAccess,
                UserId = dto.UserId
            };

            _context.Admins.Add(entity);
            await _context.SaveChangesAsync();

            return new ResponseAdminDTO
            {
                Id = entity.Id,
                UserName = entity.UserName,
                ContactNumber = entity.ContactNumber,
                Description = entity.Description,
                TypeAccess = entity.TypeAccess,
                UserId = entity.UserId
            };
        }

        public async Task<ResponseAdminDTO> Update(Guid id, UpdateAdminDTO dto)
        {
            var data = await _context.Admins.FirstOrDefaultAsync(c => c.Id == id);

            if (data == null)
                throw new Exception("Admin not found");

            data.UserName = dto.UserName;
            data.ContactNumber = dto.ContactNumber;
            data.Description = dto.Description;
            data.TypeAccess = dto.TypeAccess;
            data.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ResponseAdminDTO
            {
                Id = data.Id,
                UserName = data.UserName,
                ContactNumber = data.ContactNumber,
                Description = data.Description,
                TypeAccess = data.TypeAccess,
                UserId = data.UserId
            };
        }

        public async Task Delete(Guid id)
        {
            var data = await _context.Admins.FirstOrDefaultAsync(c => c.Id == id);

            if (data == null)
                throw new Exception("Admin not found");

            data.IsActive = false;
            data.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
