using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IMappers;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Application.Storage;
using API_PI_Clubes.Infrastructure.Extensions;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.ValueObjects;

namespace API_PI_Clubes.Application.Services
{
    public class ClubService : IClubService
    {
        private readonly IClubRepository _repository;
        private readonly IClubMapper _mapper;
        private readonly IStorageService _storageService;

        public ClubService(IClubMapper mapper, IClubRepository repository, IStorageService storageService)
        {
            _mapper = mapper;
            _repository = repository;
            _storageService = storageService;
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

        public async Task Upload(Guid id, UploadImageDTO dto)
        {
            ValidateId(id);

            if (dto == null)
                throw new InvalidOperationException("Dto Null");

            if (dto.Images == null || !dto.Images.Any())
                throw new InvalidOperationException("No images provided");

            var data = await _repository.GetByIdAsync(id);

            if (data == null)
                throw new InvalidOperationException("Id not found");

            data.Images ??= new List<Image>();

            var uploadTasks = dto.Images.Select(async file =>
            {
                if (file == null)
                    throw new InvalidOperationException("File Null");

                var extension = Path.GetExtension(file.FileName);
                var uniqueFileName = $"{Guid.NewGuid()}{extension}";

                using var stream = file.OpenReadStream();
                var imageUrl = await _storageService.UploadFileAsync(stream, uniqueFileName);

                return new Image { Url = imageUrl, Name = uniqueFileName };
            }).ToList();

            var uploadedImages = await Task.WhenAll(uploadTasks);

            data.Images.AddRange(uploadedImages);

            _repository.Update(data);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteImages(Guid id, DeleteImageDto dto)
        {
            ValidateId(id);

            if (dto == null)
                throw new InvalidOperationException("Dto Null");

            if (dto.ImageIds == null || !dto.ImageIds.Any())
                throw new InvalidOperationException("No images provided");

            var data = await _repository.GetByIdAsync(id);
            if (data == null)
                throw new InvalidOperationException("Id not found");

            if (data.Images == null || !data.Images.Any())
                throw new InvalidOperationException("Club has no images");

            var imagesToDelete = data.Images
                .Where(i => dto.ImageIds.Contains(i.ClubId))
                .ToList();

            if (!imagesToDelete.Any())
                throw new InvalidOperationException("No matching images found");

            foreach (var img in imagesToDelete)
            {
                await _storageService.DeleteFileAsync(img.Name);
            }

            foreach (var img in imagesToDelete)
                data.Images.Remove(img);

            _repository.Update(data);
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