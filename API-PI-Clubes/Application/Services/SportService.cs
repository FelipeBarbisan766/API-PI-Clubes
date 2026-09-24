using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;

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
        public async Task<List<ResponseSportDTO>> GetByIds(List<Guid> ids,CancellationToken cancellationToken)
        {
            if (ids == null || ids.Count == 0)
                return new List<ResponseSportDTO>();

            var sports = await _repository.GetByIdsAsync(ids.Distinct().ToList(),cancellationToken); 
            return sports;
        }
    }
}