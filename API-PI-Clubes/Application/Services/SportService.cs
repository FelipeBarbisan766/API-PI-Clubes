using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Exceptions;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Services
{
    public class SportService : ISportService
    {
        private readonly ISportRepository _repository;
        private readonly IUserRepository _userRepository;

        public SportService(ISportRepository repository, IUserRepository userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }

        public async Task<List<ResponseSportDTO>> GetAll(CancellationToken cancellationToken)
        {
            return await _repository.GetAllAsync(cancellationToken);
        }
        public async Task<List<ResponseSportDTO>> GetByIds(List<Guid> ids,CancellationToken cancellationToken)
        {
            if (ids == null || ids.Count == 0)
                return new List<ResponseSportDTO>();

            var sports = await _repository.GetByIdsAsync(ids.Distinct().ToList(),cancellationToken); 
            return sports;
        }

        public async Task<ResponseIdDTO> Create(Guid userId, CreateSportDTO dto, CancellationToken cancellationToken)
        {
            await AuthorizeRoot(userId, cancellationToken);
            var sport = new Sport
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(sport, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return new ResponseIdDTO { Id = sport.Id };
        }
        public async Task Delete(Guid userId, Guid id, CancellationToken cancellationToken)
        {
            ValidateId(id);
            await AuthorizeRoot(userId, cancellationToken);

            if (!await _repository.ExistsAsync(id, cancellationToken))
                throw new NotFoundException("Sport", id);

            await _repository.DeleteAsync(id, cancellationToken);
        }
        private static void ValidateId(Guid id)
        {
            if (id == Guid.Empty)
                throw new ValidationException("O ID informado é inválido.");
        }

        private async Task AuthorizeRoot(Guid userId, CancellationToken cancellationToken)
        {
            var isRoot = await _userRepository.IsRootAsync(userId, cancellationToken);
            if (!isRoot)
                throw new ForbiddenException("Você não tem permissão para gerenciar.");

        }

    }
}