using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IMappers;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Application.Storage;
using API_PI_Clubes.Infrastructure.Extensions;
using API_PI_Clubes.Infrastructure.Repositories;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.Enums;


namespace API_PI_Clubes.Application.Services
{
    public class CourtService : ICourtService
    {
        private readonly ICourtRepository _repository;
        private readonly ICourtMapper _mapper;
        private readonly IStorageService _storageService;
        private readonly IImageRepository _imageRepository;
        private readonly IImageProcessingService _imageProcessor;

        public CourtService(ICourtMapper mapper, 
            ICourtRepository repository, 
            IStorageService storageService, 
            IImageRepository imageRepository,
            IImageProcessingService imageProcessor)
        {
            _mapper = mapper;
            _repository = repository;
            _storageService = storageService;
            _imageRepository = imageRepository;
            _imageProcessor = imageProcessor;
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

            var courtId = Guid.NewGuid();
            
            var imageEntities = new List<Image>();
            if (dto.Images != null && dto.Images.Count > 0)
            {
                var uploadTasks = dto.Images.Select(file => ProcessAndUploadImage(file, courtId));
                var uploaded    = await Task.WhenAll(uploadTasks);
                imageEntities.AddRange(uploaded);
            }
            
            var entity = new Court
            {
                Id = courtId,
                Name = dto.Name,
                Type = dto.Type,
                Surface = dto.Surface,
                IsCovered = dto.IsCovered,
                PricePerHour = dto.PricePerHour,
                Description = dto.Description,
                ClubId = dto.ClubId,
                CreatedAt = DateTime.UtcNow,
                Images = imageEntities
            };
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
            var entity = await _repository.GetByIdWithImagesAsync(id);
            if (entity == null) throw new InvalidOperationException("Court not found");

            var uploadTasks = dto.Images.Select(file => ProcessAndUploadImage(file, id));
            var uploaded    = await Task.WhenAll(uploadTasks);


            foreach (var img in uploaded)
                _imageRepository.Add(img);

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
        
        private async Task<Image> ProcessAndUploadImage(IFormFile file, Guid courtId)
        {
            using var inputStream = file.OpenReadStream();
            using var result = await _imageProcessor.ProcessAsync(inputStream);

            var urls = new Dictionary<ImageVariantType, string>();

            // Upload de cada variante (podem ser paralelos se quiser velocidade extra)
            foreach (var variant in result.Variants)
            {
                urls[variant.Variant] = await _storageService.UploadFileAsync(
                    variant.Stream,
                    variant.FileName
                );
            }

            return new Image
            {
                Name      = result.BaseName,
                ThumbUrl  = urls[ImageVariantType.Thumb],
                MediumUrl = urls[ImageVariantType.Medium],
                FullUrl   = urls[ImageVariantType.Full],
                CourtId    = courtId
            };
        }   
    }
}
