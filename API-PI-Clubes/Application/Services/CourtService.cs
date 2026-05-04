using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IMappers;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Application.Storage;
using API_PI_Clubes.Infrastructure.Extensions;
using API_PI_Clubes.Infrastructure.Repositories;
using API_PI_Clubes.Model;


namespace API_PI_Clubes.Application.Services
{
    public class CourtService : ICourtService
    {
        private readonly ICourtRepository _repository;
        private readonly ICourtMapper _mapper;
        private readonly IStorageService _storageService;
        private readonly IImageRepository _imageRepository;

        public CourtService(ICourtMapper mapper, ICourtRepository repository, IStorageService storageService, IImageRepository imageRepository )
        {
            _mapper = mapper;
            _repository = repository;
            _storageService = storageService;
            _imageRepository = imageRepository;
        }

        public async Task<IEnumerable<ResponseCourtDTO>> GetAll()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<ResponseCourtDTO> GetById(Guid id)
        {
            ValidateId(id);

            var data = await _repository.GetByIdAsync(id);

            if (data == null)
                throw new InvalidOperationException("Court not found");

            return _mapper.ToDTO(data);
        }
        public async Task<List<ResponseCourtDTO>> GetByClubId(Guid id)
        {
            ValidateId(id);
            var data = await _repository.GetAllByClubIdAsync(id);
            if (data == null)
                throw new InvalidOperationException("Court not found");

            return data;
        }
        public async Task<ResponseIdDTO> Create(CreatCourtDTO dto)
        {
            ValidateCourtDTO(dto);

            var entity = new Court
            {
                Name = dto.Name,
                Type = dto.Type,
                Surface = dto.Surface,
                IsCovered = dto.IsCovered,
                PricePerHour = dto.PricePerHour,
                Description = dto.Description,
                ClubId = dto.ClubId,
                CreatedAt = DateTime.UtcNow,
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

            return new ResponseIdDTO { Id = entity.Id };
            
        }


        public async Task<ResponseCourtDTO> Update(Guid id, UpdateCourtDTO dto)
        {
            ValidateId(id);
            ValidateUpdateCourtDTO(dto);

            var data = await _repository.GetByIdAsync(id);

            if (data == null)
                throw new InvalidOperationException("Court not found");

            data.Name = dto.Name;
            data.Type = dto.Type;
            data.Surface = dto.Surface;
            data.IsCovered = dto.IsCovered;
            data.PricePerHour = dto.PricePerHour;
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
                throw new InvalidOperationException("Court not found");

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
                    CourtId = id
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

        private static void ValidateCourtDTO(CreatCourtDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
        }

        private static void ValidateUpdateCourtDTO(UpdateCourtDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
        }
    }
}
