using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Exceptions;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Application.Services;

public class ClubReviewService : IClubReviewService
{
    private readonly IClubReviewRepository _repository;
    private readonly IClubRepository _clubRepository;
    private readonly IPlayerRepository _playerRepository;

    public ClubReviewService(
        IClubReviewRepository repository,
        IClubRepository clubRepository,
        IPlayerRepository playerRepository)
    {
        _repository = repository;
        _clubRepository = clubRepository;
        _playerRepository = playerRepository;
    }

    public async Task<ResponseClubReviewSummaryDTO> RateClub(Guid userId, Guid clubId, CreateClubReviewDTO dto)
    {
        if (clubId == Guid.Empty)
            throw new ValidationException("O ID do clube informado é inválido.");

        var clubExists = await _clubRepository.ExistsAsync(clubId);
        if (!clubExists)
            throw new NotFoundException("Clube", clubId);

        var playerId = await _playerRepository.GetIdByUserIdAsync(userId);
        if (playerId == null)
            throw new ForbiddenException("Apenas jogadores podem avaliar clubes.");

        var alreadyReviewed = await _repository.ExistsAsync(clubId, playerId.Value);
        if (alreadyReviewed)
            throw new ConflictException("Você já avaliou este clube.");

        var review = new ClubReview
        {
            Id = Guid.NewGuid(),
            ClubId = clubId,
            PlayerId = playerId.Value,
            Rating = dto.Rating
        };

        await _repository.AddAsync(review);

        try
        {
            await _repository.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx &&
            (sqlEx.Number == 2601 || sqlEx.Number == 2627))
        {
            throw new ConflictException("Você já avaliou este clube.");
        }

        return await _repository.GetSummaryByClubIdAsync(clubId);
    }

    public async Task<ResponseClubReviewSummaryDTO> GetSummary(Guid clubId)
    {
        if (clubId == Guid.Empty)
            throw new ValidationException("O ID do clube informado é inválido.");

        var clubExists = await _clubRepository.ExistsAsync(clubId);
        if (!clubExists)
            throw new NotFoundException("Clube", clubId);

        return await _repository.GetSummaryByClubIdAsync(clubId);
    }

    public async Task<Boolean> VerifyReview(Guid userId, Guid clubId)
    {
        var playerId = await _playerRepository.GetIdByUserIdAsync(userId);
        if (playerId == null)
            throw new ForbiddenException("Apenas jogadores podem avaliar clubes.");

        var alreadyReviewed = await _repository.ExistsAsync(clubId, playerId.Value);
        if (alreadyReviewed)
            return true;
        
        return false;
    }
}