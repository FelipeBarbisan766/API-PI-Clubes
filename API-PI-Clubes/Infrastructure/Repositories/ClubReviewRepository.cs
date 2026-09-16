using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Infrastructure.Repositories;

public class ClubReviewRepository : IClubReviewRepository
{
    private readonly AppDbContext _context;

    public ClubReviewRepository(AppDbContext context) => _context = context;

    public async Task<bool> ExistsAsync(Guid clubId, Guid playerId)
    {
        return await _context.ClubReviews
            .AnyAsync(cr => cr.ClubId == clubId && cr.PlayerId == playerId);
    }

    public async Task AddAsync(ClubReview review)
    {
        await _context.ClubReviews.AddAsync(review);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<ResponseClubReviewSummaryDTO> GetSummaryByClubIdAsync(Guid clubId)
    {
        var result = await _context.ClubReviews
            .Where(cr => cr.ClubId == clubId)
            .GroupBy(cr => 1)
            .Select(g => new ResponseClubReviewSummaryDTO
            {
                AverageRating = Math.Round(g.Average(cr => cr.Rating), 1),
                TotalReviews = g.Count()
            })
            .FirstOrDefaultAsync();

        return result ?? new ResponseClubReviewSummaryDTO { AverageRating = 0, TotalReviews = 0 };
    }
}