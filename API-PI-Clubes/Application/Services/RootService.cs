using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;

namespace API_PI_Clubes.Application.Services
{
    public class RootService : IRootService
    {
        private readonly IRootRepository _repository;

        public RootService(IRootRepository repository)
        {
            _repository = repository;
        }

        public Task<OverviewRootDTO> Overview(CancellationToken cancellationToken)
            => _repository.GetOverviewAsync(cancellationToken);
    }
}