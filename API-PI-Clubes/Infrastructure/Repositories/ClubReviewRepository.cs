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

    public async Task<bool> ExistsAsync(Guid clubId, Guid playerId, CancellationToken cancellationToken)
    {
        return await _context.ClubReviews
            .AnyAsync(cr => cr.ClubId == clubId && cr.PlayerId == playerId,cancellationToken);
    }

    public async Task AddAsync(ClubReview review, CancellationToken cancellationToken)
    {
        await _context.ClubReviews.AddAsync(review,cancellationToken );
    }

    public async Task SaveChangesAsync( CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<ResponseClubReviewSummaryDTO> GetSummaryByClubIdAsync(Guid clubId,CancellationToken cancellationToken)
    {
        var result = await _context.ClubReviews
            .Where(cr => cr.ClubId == clubId)
            .GroupBy(cr => 1)
            .Select(g => new ResponseClubReviewSummaryDTO
            {
                AverageRating = Math.Round(g.Average(cr => cr.Rating), 1),
                TotalReviews = g.Count()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return result ?? new ResponseClubReviewSummaryDTO { AverageRating = 0, TotalReviews = 0 };
    }
}