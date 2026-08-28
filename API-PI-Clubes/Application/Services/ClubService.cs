using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Exceptions;
using API_PI_Clubes.Application.Interfaces.IMappers;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Application.Storage;
using API_PI_Clubes.Infrastructure.Extensions;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.Enums;
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
        private readonly IImageProcessingService _imageProcessor;

        public ClubService(IClubMapper mapper,
            IClubRepository repository,
            IStorageService storageService,
            IImageRepository imageRepository,
            IImageProcessingService imageProcessor
        )
        {
            _mapper = mapper;
            _repository = repository;
            _storageService = storageService;
            _imageRepository = imageRepository;
            _imageProcessor = imageProcessor;
        }

        public async Task<PagedResultDTO<ResponseClubDTO>> GetAll(ClubQueryDTO query)
        {
            var (items, total) = await _repository.GetAllAsync(query);

            return new PagedResultDTO<ResponseClubDTO>
            {
                Data = items,
                TotalCount = total,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<ResponseClubByIdDTO> GetById(Guid id)
        {
            ValidateId(id);

            var data = await _repository.GetByIdAsync(id);

            if (data == null)
                throw new NotFoundException("Clube", id);

            return _mapper.ToDTOById(data);
        }

        public async Task<List<ResponseClubDTO>> GetAllByAdminId(Guid id)
        {
            ValidateId(id);

            var data = await _repository.GetAllByAdminIdAsync(id);

            if (data == null)
                throw new NotFoundException("Admin", id);

            return data;
        }

        public async Task<ResponseDashboardDTO> GetDashboard(Guid id)
        {
            ValidateId(id);

            var data = await _repository.GetDashboardAsync(id);

            if (data == null)
                throw new NotFoundException("Clube", id);

            return data;
        }

        public async Task<ResponseIdDTO> Create(CreateClubDTO dto)
        {
            ValidateClubDTO(dto);

            var clubId = Guid.NewGuid();

            var imageEntities = new List<Image>();
            if (dto.Images != null && dto.Images.Count > 0)
            {
                var uploadTasks = dto.Images.Select(file => ProcessAndUploadImage(file, clubId));
                var uploaded    = await Task.WhenAll(uploadTasks);
                for (int i = 0; i < uploaded.Length; i++)
                    uploaded[i].Order = i;
                imageEntities.AddRange(uploaded);
            }


            var entity = new Club
            {
                Id = clubId,
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
                Images = imageEntities
            };


            var clubAdmin = new ClubAdmin { ClubId = entity.Id, AdminId = dto.adminId };
            await _repository.AddAsync(entity);
            await _repository.AddClubAdminAsync(clubAdmin);
            await _repository.SaveChangesAsync();

            return new ResponseIdDTO { Id = entity.Id };
        }

        public async Task<ResponseClubDTO> Update(Guid userId, Guid id, UpdateClubDTO dto)
        {
            ValidateId(id);
            ValidateUpdateClubDTO(dto);
            await AuthorizeOwnership(userId, id);

            var data = await _repository.GetByIdAsync(id);
            if (data == null)
                throw new Exception("Club not found");

            data.Name = dto.Name;
            data.PhoneNumber = dto.PhoneNumber;
            data.Address = new AddressVO(
                dto.ZipCode, dto.Street, dto.Number, dto.Neighborhood,
                dto.Complement, dto.City, dto.State, dto.Country
            );
            data.Description = dto.Description;
            data.UpdatedAt = DateTime.UtcNow;

            _repository.Update(data);
            await _repository.SaveChangesAsync();
            return _mapper.ToDTO(data);
        }

        public async Task Delete(Guid userId, Guid id)
        {
            ValidateId(id);
            await AuthorizeOwnership(userId, id);

            var exists = await _repository.ExistsAsync(id);
            if (!exists)
                throw new NotFoundException("Clube", id);

            await _repository.DeleteAsync(id);
        }

        public async Task AddMoreImagesAsync(Guid userId, Guid id, UploadImageDTO dto)
        {
            ValidateId(id);
            await AuthorizeOwnership(userId, id);

            var entity = await _repository.GetByIdWithImagesAsync(id);
            if (entity == null)
                throw new NotFoundException("Clube", id);

            var currentCount = entity.Images?.Count ?? 0;
            if (currentCount + dto.Images.Count > 5)
                throw new ValidationException("O clube pode ter no máximo 5 imagens.");

            var uploadTasks = dto.Images.Select(file => ProcessAndUploadImage(file, id));
            var uploaded = await Task.WhenAll(uploadTasks);

            var nextOrder = currentCount == 0 ? 0 : entity.Images.Max(i => i.Order) + 1;
            foreach (var img in uploaded)
            {
                img.Order = nextOrder++;
                _imageRepository.Add(img);
            }

            await _repository.SaveChangesAsync();
        }

        public async Task DeleteImageAsync(Guid userId, Guid id, Guid imageId)
        {
            ValidateId(id);
            await AuthorizeOwnership(userId, id);

            var entity = await _repository.GetByIdWithImagesAsync(id);
            if (entity == null)
                throw new NotFoundException("Clube", id);

            var image = entity.Images?.FirstOrDefault(i => i.Id == imageId);
            if (image == null)
                return;

            await DeleteImageFilesAsync(image);

            _imageRepository.Remove(image);
            await _repository.SaveChangesAsync();
        }

        public async Task ReorderImagesAsync(Guid userId, Guid id, List<ReorderImageDTO> orders)
        {
            ValidateId(id);
            if (orders == null || orders.Count == 0)
                throw new ValidationException("A lista de ordenação não pode ser vazia.");

            await AuthorizeOwnership(userId, id);

            var entity = await _repository.GetByIdWithImagesAsync(id);
            if (entity == null)
                throw new NotFoundException("Clube", id);

            var imagesById = entity.Images?.ToDictionary(i => i.Id) ?? new Dictionary<Guid, Image>();

            foreach (var order in orders)
            {
                if (imagesById.TryGetValue(order.Id, out var image))
                    image.Order = order.Order;
            }

            await _repository.SaveChangesAsync();
        }

        private static string ExtractFileName(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return string.Empty;
            return Path.GetFileName(new Uri(url).LocalPath);
        }

        private async Task DeleteImageFilesAsync(Image image)
        {
            try
            {
                await Task.WhenAll(
                    _storageService.DeleteFileAsync(ExtractFileName(image.ThumbUrl)),
                    _storageService.DeleteFileAsync(ExtractFileName(image.MediumUrl)),
                    _storageService.DeleteFileAsync(ExtractFileName(image.FullUrl))
                );
            }
            catch
            {
            }
        }

        private static void ValidateId(Guid id)
        {
            if (id == Guid.Empty)
                throw new ValidationException("O ID informado é inválido.");
        }

        private async Task AuthorizeOwnership(Guid userId, Guid id)
        {
            var isOwner = await _repository.IsOwnedByUserAsync(id, userId);
            if (!isOwner)
                throw new ForbiddenException("Você não tem permissão para gerenciar este clube.");
        }

        private static void ValidateClubDTO(CreateClubDTO dto)
        {
            if (dto == null)
                throw new ValidationException(nameof(dto));
        }

        private static void ValidateUpdateClubDTO(UpdateClubDTO dto)
        {
            if (dto == null)
                throw new ValidationException(nameof(dto));
        }

        private async Task<Image> ProcessAndUploadImage(IFormFile file, Guid clubId)
        {
            using var inputStream = file.OpenReadStream();
            using var result = await _imageProcessor.ProcessAsync(inputStream);

            var urls = new Dictionary<ImageVariantType, string>();
            foreach (var variant in result.Variants)
            {
                urls[variant.Variant] = await _storageService.UploadFileAsync(
                    variant.Stream,
                    variant.FileName);
            }

            return new Image
            {
                Name = result.BaseName,
                ThumbUrl = urls[ImageVariantType.Thumb],
                MediumUrl = urls[ImageVariantType.Medium],
                FullUrl = urls[ImageVariantType.Full],
                ClubId = clubId
            };
        }
    }
}