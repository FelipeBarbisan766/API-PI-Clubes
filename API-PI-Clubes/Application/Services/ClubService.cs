using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IMappers;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Application.Storage;
using API_PI_Clubes.Infrastructure.Extensions;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Application.Services
{
    public class ClubService : IClubService
    {
        private readonly IClubRepository _repository;
        private readonly IClubMapper _mapper;
        private readonly IStorageService _storageService;
        private readonly IImageRepository _imageRepository;

        public ClubService(IClubMapper mapper, IClubRepository repository, IStorageService storageService, IImageRepository imageRepository)
        {
            _mapper = mapper;
            _repository = repository;
            _storageService = storageService;
            _imageRepository = imageRepository;
        }

        public async Task<IEnumerable<ResponseClubDTO>> GetAll()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<ResponseClubByIdDTO> GetById(Guid id)
        {
            ValidateId(id);

            var data = await _repository.GetByIdAsync(id);

            if (data == null)
                throw new InvalidOperationException("Club not found");

            return _mapper.ToDTOById(data);
        }
        
        public async Task<List<ResponseClubDTO>> GetAllByAdminId(Guid id)
        {
            ValidateId(id);

            var data = await _repository.GetAllByAdminIdAsync(id);

            if (data == null)
                throw new InvalidOperationException("Club not found");

            return data;
        }
        

        public async Task<ResponseIdDTO> Create(CreateClubDTO dto)
        {
            ValidateClubDTO(dto);

            
            var entity = new Club
            {
                Name = dto.Name,
                PhoneNumber = dto.PhoneNumber,
                Address = new AddressVO(
                    dto.ZipCode,
                    dto.Street,
                    dto.Number,
                    dto.Neighborhood,
                    dto.Complement,
                    dto.City,
                    dto.State,
                    dto.Country
                ),
                Description = dto.Description,
                Images = new List<Image>() 
            };

            var uploadTasks = dto.Images.Select(async file =>
            {
                var extension = Path.GetExtension(file.FileName);
                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                
                using var stream = file.OpenReadStream();
                var imageUrl = await _storageService.UploadFileAsync(stream, uniqueFileName);

                return new Image { Url = imageUrl, Name = uniqueFileName };
            }).ToList();

            var uploadedImages = await Task.WhenAll(uploadTasks);
            entity.Images.AddRange(uploadedImages);

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            var clubAdmin = new ClubAdmin
            {
                ClubId = entity.Id,
                AdminId = dto.adminId
            };

            await _repository.AddClubAdminAsync(clubAdmin);
            await _repository.SaveChangesAsync();

            return new ResponseIdDTO { Id = entity.Id };
        }
        public async Task<ResponseClubDTO> Update(Guid id, UpdateClubDTO dto)
        {
            ValidateId(id);
            ValidateUpdateClubDTO(dto);

            var data = await _repository.GetByIdAsync(id);

            if (data == null)
                throw new InvalidOperationException("Club not found");

            data.Name = dto.Name;
            data.PhoneNumber = dto.PhoneNumber;
            data.Address = new AddressVO(
                dto.ZipCode,
                dto.Street,
                dto.Number,
                dto.Neighborhood,
                dto.Complement,
                dto.City,
                dto.State,
                dto.Country
            );
            data.Description = dto.Description;
            data.UpdatedAt = DateTime.UtcNow;

            _repository.Update(data);
            await _repository.SaveChangesAsync();

            return _mapper.ToDTO(data);
        }

        public async Task Delete(Guid id)
        {
            ValidateId(id);

            var exists = await _repository.ExistsAsync(id);

            if (!exists)
                throw new InvalidOperationException("Club not found");

            await _repository.DeleteAsync(id);
        }

        public async Task AddMoreImagesAsync(Guid id, UploadImageDTO dto)
        { 
            ValidateId(id);
            
            var entity = await _repository.GetByIdWithImagesAsync(id);
            if (entity == null) throw new Exception("Entity Not Found");


            var uploadTasks = dto.Images.Select(async file =>
            {
                var extension = Path.GetExtension(file.FileName);
                var uniqueFileName = $"{Guid.NewGuid()}{extension}";

                using var stream = file.OpenReadStream();
                var imageUrl = await _storageService.UploadFileAsync(stream, uniqueFileName);

                return new Image
                {
                    Name = uniqueFileName,
                    Url = imageUrl,
                    ClubId = id
                };
            }).ToList();

            var uploadedImages = await Task.WhenAll(uploadTasks);

            
            foreach (var img in uploadedImages)
            {
                _imageRepository.Add(img);
            }

            await _repository.SaveChangesAsync();
        }

        private static void ValidateId(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid ID", nameof(id));
        }

        private static void ValidateClubDTO(CreateClubDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
        }

        private static void ValidateUpdateClubDTO(UpdateClubDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
        }
        
    }
}