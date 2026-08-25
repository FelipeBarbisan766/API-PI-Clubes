using API_PI_Clubes.Application.Exceptions;
using API_PI_Clubes.Application.Interfaces.IMappers;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Application.Storage;

namespace API_PI_Clubes.Application.Services
{
    public class ImageService : IImageService
    {
        private readonly IImageRepository _repository;
        private readonly IStorageService _storageService;

        public ImageService(IImageRepository repository, IStorageService storageService)
        {
            _repository = repository;
            _storageService = storageService;
        }
        public async Task<bool> DeleteImageAsync(Guid userId, string fileName)
        {
            
            var imageEntity = await _repository.GetByNameAsync(fileName);
            if (imageEntity == null) return false;

            var isOwner = await _repository.IsOwnedByUserAsync(imageEntity.Id, userId);
            if (!isOwner)
                throw new ForbiddenException("Você não tem permissão para gerenciar esta imagem.");
            
            var storageDeleted = await _storageService.DeleteFileAsync(fileName);

            if (storageDeleted)
            {
                _repository.Remove(imageEntity);

                return await _repository.SaveChangesAsync();
            }

            return false;
        }
    }
}
