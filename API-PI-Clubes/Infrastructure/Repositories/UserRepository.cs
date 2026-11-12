using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.Enums;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<UserDTO>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Users
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .Select(s => new UserDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    Email = s.Email,
                    PhoneNumber = s.PhoneNumber,
                    Role = s.Role,
                    HasPassword = false,
                    AvatarUrl = s.AvatarUrl
                })
                .ToListAsync(cancellationToken);
        }
        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive, cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken)
        {
            await _context.Users.AddAsync(user, cancellationToken);
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
        }

        public async Task<bool> ExistsByCpfHashAsync(string cpfHash, CancellationToken cancellationToken)
        {
            return await _context.Users.AnyAsync(u => u.CpfHash == cpfHash, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public Task<bool> IsRootAsync(Guid userId, CancellationToken cancellationToken)
        {
            return _context.Users.AnyAsync(u => u.Id == userId && u.Role == RoleEnum.Root, cancellationToken);
        }
    }
}