using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Email;
using API_PI_Clubes.Application.Interfaces.IMappers;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Application.Storage;
using API_PI_Clubes.Infrastructure.Security.Interfaces;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserMapper _mapper;
        private readonly IImageProcessingService _imageProcessor;
        private readonly IStorageService _storageService;
        private readonly IHttpClientFactory  _httpClientFactory;

        public UserService(
            IUserRepository userRepository, 
            IPasswordHasher passwordHasher, 
            IUserMapper mapper,
            IImageProcessingService imageProcessor,
            IStorageService storageService,
            IHttpClientFactory  httpClientFactory
            )
        {
            _repository = userRepository;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
            _imageProcessor = imageProcessor;
            _storageService = storageService;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ResponseUserDTO> GetById(Guid id)
        {
            var user = await _repository.GetByIdAsync(id);

            if (user == null)
                throw new Exception("User not found");

            return _mapper.ToDTO(user);
        }


        public async Task<ResponseUserDTO> Update(Guid id, UpdateUserDTO dto)
        {
            var user = await _repository.GetByIdAsync(id);

            if (user == null)
                throw new Exception("User not found");

            if(dto.Name != null)
                user.Name = dto.Name;
            if (dto.PhoneNumber != null)
                user.PhoneNumber = dto.PhoneNumber;
            user.UpdatedAt = DateTime.UtcNow;

            _repository.Update(user);
            await _repository.SaveChangesAsync();

            return _mapper.ToDTO(user);
        }

        public async Task UpdateRole(Guid id, RoleEnum role)
        {
            var user = await _repository.GetByIdAsync(id);

            if (user == null)
                throw new Exception("User not found");

            user.Role = RoleEnum.Admin;
            user.UpdatedAt = DateTime.UtcNow;

            _repository.Update(user);
            await _repository.SaveChangesAsync();
        }
        
        public async Task UpdateAvatar(Guid id, UpdateAvatarDTO dto)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
                throw new Exception("User not found");

            if (dto.AvatarImage != null)
            {
                var oldAvatarUrl = user.AvatarUrl;

                user.AvatarUrl = await ProcessAndUploadImage(dto.AvatarImage);
                user.UpdatedAt = DateTime.UtcNow;

                _repository.Update(user);
                await _repository.SaveChangesAsync();

                if (!string.IsNullOrEmpty(oldAvatarUrl))
                {
                    var oldFileName = Path.GetFileName(new Uri(oldAvatarUrl).LocalPath);
                    await _storageService.DeleteFileAsync(oldFileName);
                }
            }
        }

        public async Task Delete(Guid id)
        {
            var user = await _repository.GetByIdAsync(id);

            if (user == null)
                throw new Exception("User not found");

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            _repository.Update(user);
            await _repository.SaveChangesAsync();
        }
        
        private async Task<string> ProcessAndUploadImage(IFormFile file)
        {
            using var inputStream = file.OpenReadStream();
            return await ProcessAndUploadImageStream(inputStream);
        }
        private async Task<string> ProcessAndUploadImageStream(Stream inputStream)
        {
            using var variant = await _imageProcessor.ProcessAsync(inputStream, ImageVariantType.Avatar);
            return await _storageService.UploadFileAsync(variant.Stream, variant.FileName);
        }
        public async Task<string> ProcessAvatarFromUrlAsync(string imageUrl)
        {
            using var httpClient = _httpClientFactory.CreateClient();
            await using var inputStream = await httpClient.GetStreamAsync(imageUrl);
            return await ProcessAndUploadImageStream(inputStream);
        }
    }
}
