using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Infrastructure.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly AppDbContext _context;

        public ImageRepository(AppDbContext context)
        {
            _context = context;
    }

        public async Task<Image?> GetByNameAsync(string fileName)
        {
            return await _context.Images
                .FirstOrDefaultAsync(x => x.Name == fileName);
        }

        public void Remove(Image image)
        {
            _context.Images.Remove(image);
        }

        public void Add(Image image)
        {
            _context.Images.Add(image);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> IsOwnedByUserAsync(Guid Id, Guid userId)
        {
            return await _context.Images
                .AnyAsync(c => c.Id == Id && c.Club.ClubAdmin.Any(a => a.Admin.UserId == userId) || c.Court.Club.ClubAdmin.Any(a => a.Admin.UserId == userId));
        }
    }
}
