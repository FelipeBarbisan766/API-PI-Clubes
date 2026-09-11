using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Exceptions;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;

namespace API_PI_Clubes.Application.Services;

public class PlanLimitService : IPlanLimitService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IClubRepository _clubRepository;
    private readonly ICourtRepository _courtRepository;

    public PlanLimitService(
        ISubscriptionRepository subscriptionRepository,
        IClubRepository clubRepository,
        ICourtRepository courtRepository)
    {
        _subscriptionRepository = subscriptionRepository;
        _clubRepository = clubRepository;
        _courtRepository = courtRepository;
    }

    public async Task EnsureClubLimitNotReachedAsync(Guid adminId)
    {
        var limits = await GetLimitsOrThrowAsync(adminId);
        var current = await _clubRepository.CountByAdminIdAsync(adminId);

        if (current >= limits.QuantClub)
            throw new PlanLimitExceededException("clube", limits.QuantClub);
    }

    public async Task EnsureCourtLimitNotReachedAsync(Guid userId, Guid clubId)
    {
        var limits = await GetLimitsOrThrowAsync(userId,clubId);
        var current = await _courtRepository.CountByClubIdAsync(clubId);

        if (current >= limits.QuantCourt)
            throw new PlanLimitExceededException("quadra", limits.QuantCourt);
    }

    private async Task<PlanLimitsDTO> GetLimitsOrThrowAsync(Guid adminId)
    {
        var limits = await _subscriptionRepository.GetActivePlanLimitsByAdminIdAsync(adminId);
        if (limits == null)
            throw new NoActiveSubscriptionException();

        return limits;
    }
    private async Task<PlanLimitsDTO> GetLimitsOrThrowAsync(Guid userId, Guid clubId)
    {
        var limits = await _subscriptionRepository.GetActivePlanLimitsByUserIdAsync(userId);
        if (limits == null)
            throw new NoActiveSubscriptionException();

        return limits;
    }
    public async Task<PlanUsageDTO> GetUsageSummaryAsync(Guid adminId)
    {
        var limits = await GetLimitsOrThrowAsync(adminId);
        var clubsUsed = await _clubRepository.CountByAdminIdAsync(adminId);
        var courtUsage = await _clubRepository.GetClubsWithCourtCountByAdminIdAsync(adminId);

        return new PlanUsageDTO
        {
            PlanName = limits.PlanName,
            Clubs = new ResourceUsageDTO { Used = clubsUsed, Limit = limits.QuantClub },
            CourtLimitPerClub = limits.QuantCourt,
            ClubsCourtUsage = courtUsage.Select(c => new ClubCourtUsageDTO
            {
                ClubId = c.ClubId,
                ClubName = c.ClubName,
                Used = c.Used,
                Limit = limits.QuantCourt
            }).ToList()
        };
    }
}