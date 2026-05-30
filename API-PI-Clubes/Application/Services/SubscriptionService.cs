using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IMappers;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.Enums;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
     
            if (subscription is null)
                return null;
     
            return MapToDto(subscription);
        }
     
        public async Task<bool> CheckAccessAsync(Guid adminId)
        {
            var subscription = await _subscriptionRepository.GetActiveByAdminIdAsync(adminId);
     
            if (subscription is null)
                return false;
     
            return subscription.IsActive && subscription.ExpiresAt > DateTime.UtcNow;
        }
     
        public async Task RenewAsync(Guid adminId, Guid paymentId)
        {
            var current = await _subscriptionRepository.GetActiveByAdminIdAsync(adminId);
     
            if (current is null)
                throw new Exception("Nenhuma assinatura ativa encontrada para renovar.");
     
            var plan = await _planRepository.GetByIdAsync(current.PlanId)
                ?? throw new Exception("Plano da assinatura não encontrado.");
     
            current.PaymentId = paymentId;
            current.StartDate = current.ExpiresAt;
            current.ExpiresAt = current.ExpiresAt.AddDays(plan.DurationDays);
            current.IsActive = true;
     
            await _subscriptionRepository.UpdateAsync(current);
        }
     
        public async Task CancelAsync(Guid subscriptionId)
        {
            var subscription = await _subscriptionRepository.GetByIdAsync(subscriptionId)
                ?? throw new Exception("Assinatura não encontrada.");
     
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
