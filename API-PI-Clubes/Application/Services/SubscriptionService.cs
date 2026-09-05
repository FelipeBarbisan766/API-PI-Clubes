using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Model;
using API_PI_Clubes.Application.Exceptions;

namespace API_PI_Clubes.Application.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IPlanRepository _planRepository;

        public SubscriptionService(
            ISubscriptionRepository subscriptionRepository,
            IPlanRepository planRepository
        )
        {
            _subscriptionRepository = subscriptionRepository;
            _planRepository = planRepository;
        }

        public async Task<SubscriptionResponseDto?> GetActiveByAdminAsync(Guid adminId)
        {
            var subscription = await _subscriptionRepository.GetActiveByAdminIdAsync(adminId);
            return subscription is null ? null : MapToDto(subscription);
        }

        public async Task<bool> CheckAccessAsync(Guid adminId)
        {
            var subscription = await _subscriptionRepository.GetActiveByAdminIdAsync(adminId);
            if (subscription is null) return false;
            return subscription.IsActive && subscription.ExpiresAt > DateTime.UtcNow;
        }

        public async Task RenewAsync(Guid adminId, Guid paymentId)
        {
            var current = await _subscriptionRepository.GetActiveByAdminIdAsync(adminId);
            if (current is null)
                throw new NotFoundException("Assinatura ativa para este admin");

            var plan = await _planRepository.GetByIdAsync(current.PlanId)
                       ?? throw new NotFoundException("Plano", current.PlanId);

            current.PaymentId = paymentId;
            current.StartDate = current.ExpiresAt;
            current.ExpiresAt = current.ExpiresAt.AddDays(plan.DurationDays);
            current.IsActive = true;

            await _subscriptionRepository.UpdateAsync(current);
        }

        public async Task CancelAsync(Guid subscriptionId, Guid userId)
        {
            await AuthorizeOwnership(userId, subscriptionId);

            var subscription = await _subscriptionRepository.GetByIdAsync(subscriptionId)
                               ?? throw new NotFoundException("Assinatura", subscriptionId);

            subscription.IsActive = false;
            await _subscriptionRepository.UpdateAsync(subscription);
        }

        public async Task ExpireOverdueAsync()
        {
            var expired = await _subscriptionRepository.GetExpiredAsync();
            foreach (var subscription in expired)
            {
                subscription.IsActive = false;
                await _subscriptionRepository.UpdateAsync(subscription);
            }
        }

        private async Task AuthorizeOwnership(Guid userId, Guid subscriptionId)
        {
            var isOwner = await _subscriptionRepository.IsOwnedByUserAsync(subscriptionId, userId);
            if (!isOwner)
                throw new ForbiddenException("Você não tem permissão para gerenciar esta assinatura.");
        }

        private static SubscriptionResponseDto MapToDto(Subscription s) => new(
            Id: s.Id,
            AdminId: s.AdminId,
            PlanId: s.PlanId,
            PlanName: s.Plan.Name,
            StartDate: s.StartDate,
            ExpiresAt: s.ExpiresAt,
            IsActive: s.IsActive
        );
    }
}