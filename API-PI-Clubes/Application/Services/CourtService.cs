using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Exceptions;
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
        private readonly IPlanLimitService _planLimitService;

        private readonly ISportRepository _sportRepository;

        public CourtService(ICourtMapper mapper,
            ICourtRepository repository,
            IStorageService storageService,
            IImageRepository imageRepository,
            IImageProcessingService imageProcessor,
            ISportRepository sportRepository,
            IPlanLimitService planLimitService
        )
        {
            _mapper = mapper;
            _repository = repository;
            _storageService = storageService;
            _imageRepository = imageRepository;
            _imageProcessor = imageProcessor;
            _sportRepository = sportRepository;
            _planLimitService = planLimitService;
        }

        public async Task<PagedResultDTO<ResponseCourtDTO>> GetAll(CourtQueryDTO query,
            CancellationToken cancellationToken)
        {
            var (items, total) = await _repository.GetAllAsync(query, cancellationToken);

            return new PagedResultDTO<ResponseCourtDTO>
            {
                Data = items,
                TotalCount = total,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<ResponseCourtDTO> GetById(Guid id, CancellationToken cancellationToken)
        {
            ValidateId(id);

            var data = await _repository.GetByIdAsync(id, cancellationToken);

            if (data == null)
                throw new NotFoundException("Quadra", id);

            return _mapper.ToDTO(data);
        }

        public async Task<List<ResponseCourtDTO>> GetByClubId(Guid id, CancellationToken cancellationToken)
        {
            ValidateId(id);
            var data = await _repository.GetAllByClubIdAsync(id, cancellationToken);
            if (data == null)
                throw new NotFoundException("Clube", id);

            return data;
        }

        public async Task<ResponseIdDTO> Create(Guid userId, CreatCourtDTO dto, CancellationToken cancellationToken)
        {
            ValidateCourtDTO(dto);
            await _planLimitService.EnsureCourtLimitNotReachedAsync(userId, dto.ClubId, cancellationToken);
            await ValidateSportIdsAsync(dto.SportIds, cancellationToken);

            var courtId = Guid.NewGuid();

            var imageEntities = new List<Image>();
            if (dto.Images != null && dto.Images.Count > 0)
            {
                var uploadTasks = dto.Images.Select(file => ProcessAndUploadImage(file, courtId, cancellationToken));
                var uploaded = await Task.WhenAll(uploadTasks);
                for (int i = 0; i < uploaded.Length; i++)
                    uploaded[i].Order = i;
                imageEntities.AddRange(uploaded);
            }

            var entity = new Court
            {
                Id = courtId,
                Name = dto.Name,
                Surface = dto.Surface,
                IsCovered = dto.IsCovered,
                PricePerHour = dto.PricePerHour,
                Description = dto.Description,
                ClubId = dto.ClubId,
                CreatedAt = DateTime.UtcNow,
                Images = imageEntities,
                CourtSports = dto.SportIds
                    .Distinct()
                    .Select(sportId => new CourtSport { CourtId = courtId, SportId = sportId })
                    .ToList()
            };

            await _repository.AddAsync(entity, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return new ResponseIdDTO { Id = entity.Id };
        }


        public async Task<ResponseCourtDTO> Update(Guid userId, Guid id, UpdateCourtDTO dto,
            CancellationToken cancellationToken)
        {
            ValidateId(id);
            ValidateUpdateCourtDTO(dto);
            await ValidateSportIdsAsync(dto.SportIds, cancellationToken);
            await AuthorizeOwnership(userId, id, cancellationToken);

            var data = await _repository.GetByIdAsync(id, cancellationToken);

            if (data == null)
                throw new NotFoundException("Quadra", id);

            data.Name = dto.Name;
            data.Surface = dto.Surface;
            data.IsCovered = dto.IsCovered;
            data.PricePerHour = dto.PricePerHour;
            data.Description = dto.Description;
            data.UpdatedAt = DateTime.UtcNow;

            SyncCourtSports(data, dto.SportIds);

            _repository.Update(data);
            await _repository.SaveChangesAsync(cancellationToken);

            return _mapper.ToDTO(data);
        }

        private static void SyncCourtSports(Court court, List<Guid> newSportIds)
        {
            var newIds = newSportIds.Distinct().ToHashSet();
            var currentIds = court.CourtSports.Select(cs => cs.SportId).ToHashSet();

            var toRemove = court.CourtSports.Where(cs => !newIds.Contains(cs.SportId)).ToList();
            foreach (var cs in toRemove)
                court.CourtSports.Remove(cs);

            var toAdd = newIds.Where(sid => !currentIds.Contains(sid));
            foreach (var sportId in toAdd)
                court.CourtSports.Add(new CourtSport { CourtId = court.Id, SportId = sportId });
        }

        public async Task Delete(Guid userId, Guid id, CancellationToken cancellationToken)
        {
            ValidateId(id);
            await AuthorizeOwnership(userId, id, cancellationToken);

            var exists = await _repository.ExistsAsync(id, cancellationToken);

            if (!exists)
                throw new NotFoundException("Quadra", id);

            await _repository.DeleteAsync(id, cancellationToken);
        }

        public async Task AddMoreImagesAsync(Guid userId, Guid id, UploadImageDTO dto,
            CancellationToken cancellationToken)
        {
            ValidateId(id);
            await AuthorizeOwnership(userId, id, cancellationToken);

            var entity = await _repository.GetByIdWithImagesAsync(id, cancellationToken);
            if (entity == null)
                throw new NotFoundException("Quadra", id);

            var currentCount = entity.Images?.Count ?? 0;
            if (currentCount + dto.Images.Count > 3)
                throw new ValidationException("A Quadra pode ter no máximo 3 imagens.");

            var uploadTasks = dto.Images.Select(file => ProcessAndUploadImage(file, id, cancellationToken));
            var uploaded = await Task.WhenAll(uploadTasks);

            var nextOrder = currentCount == 0 ? 0 : entity.Images.Max(i => i.Order) + 1;
            foreach (var img in uploaded)
            {
                img.Order = nextOrder++;
                _imageRepository.Add(img);
            }

            await _repository.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteImageAsync(Guid userId, Guid id, Guid imageId, CancellationToken cancellationToken)
        {
            ValidateId(id);
            await AuthorizeOwnership(userId, id, cancellationToken);

            var entity = await _repository.GetByIdWithImagesAsync(id, cancellationToken);
            if (entity == null)
                throw new NotFoundException("Quadra", id);

            var image = entity.Images?.FirstOrDefault(i => i.Id == imageId);
            if (image == null)
                return;

            await DeleteImageFilesAsync(image, cancellationToken);

            _imageRepository.Remove(image);
            await _repository.SaveChangesAsync(cancellationToken);
        }

        public async Task ReorderImagesAsync(Guid userId, Guid id, List<ReorderImageDTO> orders,
            CancellationToken cancellationToken)
        {
            ValidateId(id);
            if (orders == null || orders.Count == 0)
                throw new ValidationException("A lista de ordenação não pode ser vazia.");

            await AuthorizeOwnership(userId, id, cancellationToken);

            var entity = await _repository.GetByIdWithImagesAsync(id, cancellationToken);
            if (entity == null)
                throw new NotFoundException("Quadra", id);

            var imagesById = entity.Images?.ToDictionary(i => i.Id) ?? new Dictionary<Guid, Image>();

            foreach (var order in orders)
            {
                if (imagesById.TryGetValue(order.Id, out var image))
                    image.Order = order.Order;
            }

            await _repository.SaveChangesAsync(cancellationToken);
        }

        private static string ExtractFileName(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return string.Empty;
            return Path.GetFileName(new Uri(url).LocalPath);
        }

        private async Task DeleteImageFilesAsync(Image image, CancellationToken cancellationToken)
        {
            try
            {
                await Task.WhenAll(
                    _storageService.DeleteFileAsync(ExtractFileName(image.ThumbUrl), cancellationToken),
                    _storageService.DeleteFileAsync(ExtractFileName(image.MediumUrl), cancellationToken),
                    _storageService.DeleteFileAsync(ExtractFileName(image.FullUrl), cancellationToken)
                );
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch
            {
            }
        }

        private async Task AuthorizeOwnership(Guid userId, Guid id, CancellationToken cancellationToken)
        {
            var isOwner = await _repository.IsOwnedByUserAsync(id, userId, cancellationToken);
            if (!isOwner)
                throw new ForbiddenException("Você não tem permissão para gerenciar esta quadra.");
        }

        private static void ValidateId(Guid id)
        {
            if (id == Guid.Empty)
                throw new ValidationException("O ID informado é inválido.");
        }

        private static void ValidateCourtDTO(CreatCourtDTO dto)
        {
            if (dto == null)
                throw new ValidationException("Os dados da quadra são obrigatórios.");
        }

        private static void ValidateUpdateCourtDTO(UpdateCourtDTO dto)
        {
            if (dto == null)
                throw new ValidationException("Os dados de atualização são obrigatórios.");
        }

        private async Task ValidateSportIdsAsync(List<Guid> sportIds, CancellationToken cancellationToken)
        {
            if (sportIds == null || sportIds.Count == 0)
                throw new ValidationException("A quadra deve ter ao menos um esporte.");

            var distinctIds = sportIds.Distinct().ToList();
            var existingCount = await _sportRepository.CountExistingAsync(distinctIds, cancellationToken);

            if (existingCount != distinctIds.Count)
                throw new ValidationException("Um ou mais esportes informados são inválidos.");
        }

        private async Task<Image> ProcessAndUploadImage(IFormFile file, Guid courtId,
            CancellationToken cancellationToken)
        {
            using var inputStream = file.OpenReadStream();
            using var result = await _imageProcessor.ProcessAsync(inputStream);

            var urls = new Dictionary<ImageVariantType, string>();

            foreach (var variant in result.Variants)
            {
                urls[variant.Variant] = await _storageService.UploadFileAsync(
                    variant.Stream,
                    variant.FileName,
                    cancellationToken
                );
            }

            return new Image
            {
                Name = result.BaseName,
                ThumbUrl = urls[ImageVariantType.Thumb],
                MediumUrl = urls[ImageVariantType.Medium],
                FullUrl = urls[ImageVariantType.Full],
                CourtId = courtId
            };
        }
    }
}