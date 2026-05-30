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
    public class PlanService : IPlanService
    {
        private readonly IPlanRepository _planRepository;

        public PlanService(IPlanRepository planRepository)
        {
            _planRepository = planRepository;
        }

        public async Task<IEnumerable<PlanResponseDto>> GetAllActiveAsync()
        {
            var plans = await _planRepository.GetAllActiveAsync();
            return plans.Select(MapToDto);
        }
 
        public async Task<PlanResponseDto> CreateAsync(CreatePlanDto dto)
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
 
            await _planRepository.AddAsync(plan);
            return MapToDto(plan);
        }
 
        public async Task<PlanResponseDto> UpdateAsync(Guid id, UpdatePlanDto dto)
        {
            var plan = await _planRepository.GetByIdAsync(id)
                       ?? throw new Exception("Plano não encontrado.");
 
            // Só atualiza os campos que foram enviados
            if (dto.Name is not null) plan.Name = dto.Name;
            if (dto.Description is not null) plan.Description = dto.Description;
            if (dto.Price is not null) plan.Price = dto.Price.Value;
            if (dto.QuantClub is not null) plan.QuantClub = dto.QuantClub.Value;
            if (dto.QuantCourt is not null) plan.QuantCourt = dto.QuantCourt.Value;
            if (dto.DurationDays is not null) plan.DurationDays = dto.DurationDays.Value;
 
            await _planRepository.UpdateAsync(plan);
            return MapToDto(plan);
        }
 
        public async Task SetActiveAsync(Guid id, bool isActive)
        {
            var plan = await _planRepository.GetByIdAsync(id)
                       ?? throw new Exception("Plano não encontrado.");
 
            plan.IsActive = isActive;
            await _planRepository.UpdateAsync(plan);
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
