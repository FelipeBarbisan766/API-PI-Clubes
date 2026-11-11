using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Exceptions;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Application.Services
{
    public class SportService : ISportService
    {
        private readonly ISportRepository _repository;

        public SportService(ISportRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ResponseSportDTO>> GetAll(CancellationToken cancellationToken)
        {
            return await _repository.GetAllAsync(cancellationToken);
        }

        public async Task<List<ResponseSportDTO>> GetByIds(List<Guid> ids, CancellationToken cancellationToken)
        {
            if (ids == null || ids.Count == 0)
                return new List<ResponseSportDTO>();

            var sports = await _repository.GetByIdsAsync(ids.Distinct().ToList(), cancellationToken);
            return sports;
        }

        public async Task<List<ResponseSportBackofficeDTO>> GetAllForBackoffice(
            bool includeInactive, CancellationToken cancellationToken)
        {
            return await _repository.GetAllForBackofficeAsync(includeInactive, cancellationToken);
        }

        public async Task<ResponseSportBackofficeDTO> Create(SaveSportDTO dto, CancellationToken cancellationToken)
        {
            var name = dto.Name.Trim();

            if (await _repository.ExistsByNameAsync(name, null, cancellationToken))
                throw new ConflictException("Já existe um esporte com esse nome.");

            var entity = new Sport { Name = name, IsActive = true };

            await _repository.AddAsync(entity, cancellationToken);
            await SaveHandlingDuplicateAsync(cancellationToken);

            return new ResponseSportBackofficeDTO { Id = entity.Id, Name = entity.Name, IsActive = entity.IsActive };
        }

        public async Task<ResponseSportBackofficeDTO> Update(Guid id, SaveSportDTO dto,
            CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdForUpdateAsync(id, cancellationToken)
                         ?? throw new NotFoundException("Esporte não encontrado.");

            var name = dto.Name.Trim();

            if (await _repository.ExistsByNameAsync(name, id, cancellationToken))
                throw new ConflictException("Já existe um esporte com esse nome.");

            entity.Name = name;
            await SaveHandlingDuplicateAsync(cancellationToken);

            return new ResponseSportBackofficeDTO { Id = entity.Id, Name = entity.Name, IsActive = entity.IsActive };
        }

        public async Task SetActive(Guid id, bool isActive, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdForUpdateAsync(id, cancellationToken)
                         ?? throw new NotFoundException("Esporte não encontrado.");

            if (entity.IsActive == isActive) return;

            entity.IsActive = isActive;
            await _repository.SaveChangesAsync(cancellationToken);
        }

        private async Task SaveHandlingDuplicateAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _repository.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
            {
                throw new ConflictException("Já existe um esporte com esse nome.");
            }
        }
    }
}