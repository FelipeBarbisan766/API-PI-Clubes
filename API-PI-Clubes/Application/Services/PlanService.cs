using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IMappers;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.Enums;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using API_PI_Clubes.Application.Exceptions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API_PI_Clubes.Application.Services
{
    public class PlanService : IPlanService
    {
        private readonly IPlanRepository _planRepository;
        private readonly IPlanPricingCalculator _calculator;
        private readonly IAdminRepository _adminRepository;

        public PlanService(IPlanRepository planRepository,
            IPlanPricingCalculator calculator,
            IAdminRepository adminRepository)
        {
            _planRepository = planRepository;
            _calculator = calculator;
            _adminRepository = adminRepository;
        }

        public async Task<IEnumerable<PlanResponseDto>> GetAllActiveAsync(CancellationToken cancellationToken)
        {
            var plans = await _planRepository.GetAllActiveAsync(cancellationToken);
            return plans.Select(MapToDto);
        }

        public async Task<IEnumerable<PlanResponseDto>> GetMyCustomAsync(Guid userId, CancellationToken ct)
        {
            var adminId = await _adminRepository.GetIdByUserId(userId, ct)
                          ?? throw new NotFoundException("User", userId);

            var plans = await _planRepository.GetCustomByAdminAsync(adminId, ct);
            return plans.Select(MapToDto);
        }

        public async Task<PlanResponseDto> CreateAsync(CreatePlanDto dto, CancellationToken cancellationToken)
        {
            var plan = new Plan
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                QuantClub = dto.QuantClub,
                QuantCourt = dto.QuantCourt,
                DurationDays = dto.DurationDays,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _planRepository.AddAsync(plan, cancellationToken);
            return MapToDto(plan);
        }

        public async Task<PlanResponseDto> CreateCustomAsync(Guid userId, CustomPlanRequestDTO req,
            CancellationToken ct)
        {
            var adminId = await _adminRepository.GetIdByUserId(userId, ct)
                          ?? throw new NotFoundException("Admin", userId);
            var price = await _calculator.CalculateAsync(req, ct);

            var plan = new Plan
            {
                Id = Guid.NewGuid(),
                Name = $"Plano Personalizado ({req.ClubQuantity} clubes, {req.CourtQuantity} quadras)",
                Description = $"{req.DurationMonths} mês(es)",
                Price = price,
                DurationDays = req.DurationMonths * 30,
                QuantClub = req.ClubQuantity,
                QuantCourt = req.CourtQuantity,
                IsCustom = true,
                OwnerAdminId = adminId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _planRepository.AddAsync(plan, ct);
            return MapToDto(plan);
        }

        public async Task<PlanResponseDto> UpdateAsync(Guid id, UpdatePlanDto dto, CancellationToken cancellationToken)
        {
            var plan = await _planRepository.GetByIdAsync(id, cancellationToken)
                       ?? throw new NotFoundException("Plano", id);

            if (dto.Name is not null) plan.Name = dto.Name;
            if (dto.Description is not null) plan.Description = dto.Description;
            if (dto.Price is not null) plan.Price = dto.Price.Value;
            if (dto.QuantClub is not null) plan.QuantClub = dto.QuantClub.Value;
            if (dto.QuantCourt is not null) plan.QuantCourt = dto.QuantCourt.Value;
            if (dto.DurationDays is not null) plan.DurationDays = dto.DurationDays.Value;

            await _planRepository.UpdateAsync(plan, cancellationToken);
            return MapToDto(plan);
        }

        public async Task SetActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken)
        {
            var plan = await _planRepository.GetByIdAsync(id, cancellationToken)
                       ?? throw new NotFoundException("Plano", id);

            plan.IsActive = isActive;
            await _planRepository.UpdateAsync(plan, cancellationToken);
        }


        private static PlanResponseDto MapToDto(Plan p) => new(
            Id: p.Id,
            Name: p.Name,
            Description: p.Description,
            Price: p.Price,
            QuantClub: p.QuantClub,
            QuantCourt: p.QuantCourt,
            DurationDays: p.DurationDays,
            IsActive: p.IsActive
        );
    }
}