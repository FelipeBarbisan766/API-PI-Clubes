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
        private readonly ISubscriptionRepository _repository;

        public SubscriptionService(ISubscriptionRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResponseSubscriptionDTO> CheckAccess(Guid id)
        {
            return null;
        }
        public async Task<ResponseSubscriptionDTO> GetActiveByAdmin(Guid id)
        {
            return null;
        }
        public async Task<ResponseIdDTO> Create(CreatSubscriptionDTO dto)
        {
            return null;

        }


        public async Task<ResponseSubscriptionDTO> Renew(Guid id, UpdateSubscriptionDTO dto)
        {
            return null;
        }

        public async Task Cancel(Guid id)
        {
            
        }

        public async Task ExpireOverdue()
        {
        }

        private static void ValidateId(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid ID", nameof(id));
        }

        private static void ValidateSubscriptionDTO(CreatSubscriptionDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
        }

        private static void ValidateUpdateSubscriptionDTO(UpdateSubscriptionDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
        }
    }
}
