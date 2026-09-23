using System.Security.Claims;
using API_PI_Clubes.Application.Common;
using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Exceptions;
using API_PI_Clubes.Application.Interfaces.IMappers;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Security.Interfaces;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace API_PI_Clubes.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _repository;
        private readonly IAdminMapper _mapper;
        private readonly IUserService _userService;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IPlanRepository _planRepository;

        public AdminService(
            IAdminMapper mapper,
            IAdminRepository repository,
            IUserService userService,
            ISubscriptionRepository subscriptionRepository,
            IPlanRepository planRepository)
        {
            _mapper = mapper;
            _repository = repository;
            _userService = userService;
            _subscriptionRepository = subscriptionRepository;
            _planRepository = planRepository;
        }

        public async Task<ResponseAdminDTO> GetById(Guid id, CancellationToken cancellationToken)
        {
            ValidateId(id);

            var data = await _repository.GetByIdAsync(id, cancellationToken);

            if (data == null)
                throw new NotFoundException("Admin", id); 

            return _mapper.ToDTO(data);
        }
        public async Task<ResponseAdminDTO> GetCurrentUserInfo(Guid id, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByUserIdAsync(id, cancellationToken);
            if (entity == null)
                throw new NotFoundException("Usuário", id);
            return _mapper.ToDTO(entity);
            
        }
        public async Task<ResponseIdDTO> Create(Guid id, CancellationToken cancellationToken)
        {
            ValidateId(id);

            var strategy = _repository.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async ct =>
            {
                using var transaction = await _repository.BeginTransactionAsync(ct);

                try
                {
                    var user = await _userService.GetById(id, ct)
                               ?? throw new NotFoundException("Usuário", id);

                    var freePlan = await _planRepository.GetByIdAsync(PlanConstants.FreePlanId, ct)
                                   ?? throw new InvalidOperationException(
                                       "Plano Free não está configurado no banco de dados.");

                    var entity = new Admin
                    {
                        UserId = id,
                        TypeAccess = TypeAccessEnum.write,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _repository.AddAsync(entity, ct);

                    var freeSubscription = new Subscription
                    {
                        Id = Guid.NewGuid(),
                        AdminId = entity.Id,
                        PlanId = freePlan.Id,
                        PaymentId = null,
                        StartDate = DateTime.UtcNow,
                        ExpiresAt = DateTime.UtcNow.AddDays(freePlan.DurationDays),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _subscriptionRepository.AddAsync(freeSubscription, ct);

                    await _userService.UpdateRole(id, RoleEnum.Admin, ct);

                    await _repository.SaveChangesAsync(ct);
                    await transaction.CommitAsync(ct);

                    return new ResponseIdDTO { Id = entity.Id };
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(CancellationToken.None);
                    throw;
                }
            }, cancellationToken);
        }


        public async Task<ResponseAdminDTO> Update(Guid userId, Guid id, UpdateAdminDTO dto, CancellationToken cancellationToken)
        {
            ValidateId(id);
            ValidateUpdateAdminDTO(dto);
            await AuthorizeOwnership(userId, id, cancellationToken);

            var data = await _repository.GetByIdAsync(id, cancellationToken);

            if (data == null)
                throw new NotFoundException("Admin", id); 

            data.UpdatedAt = DateTime.UtcNow;

            _repository.Update(data);
            await _repository.SaveChangesAsync(cancellationToken);

            return _mapper.ToDTO(data);
        }

        public async Task Delete(Guid userId, Guid id, CancellationToken cancellationToken)
        {
            ValidateId(id);
            await AuthorizeOwnership(userId, id, cancellationToken);

            var exists = await _repository.ExistsAsync(id, cancellationToken);

            if (!exists)
                throw new NotFoundException("Admin", id); 

            await _repository.DeleteAsync(id, cancellationToken);
        }
        private async Task AuthorizeOwnership(Guid userId, Guid id, CancellationToken cancellationToken)
        {
            var isOwner = await _repository.IsOwnedByUserAsync(id, userId, cancellationToken);
            if (!isOwner)
                throw new ForbiddenException("Você não tem permissão para gerenciar este admin.");
        }
        private static void ValidateId(Guid id)
        {
            if (id == Guid.Empty)
                throw new ValidationException("O ID informado é inválido.");
        }

        private static void ValidateAdminDTO(CreatAdminDTO dto)
        {
            if (dto == null)
                throw new ValidationException("Os dados do admin são obrigatórios.");
        }

        private static void ValidateUpdateAdminDTO(UpdateAdminDTO dto)
        {
            if (dto == null)
                throw new ValidationException("Os dados de atualização são obrigatórios.");
        }
    }
}