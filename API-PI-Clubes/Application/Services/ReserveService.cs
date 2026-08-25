using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Exceptions;
using API_PI_Clubes.Application.Interfaces.IMappers;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Application.Services
{
    public class ReserveService : IReserveService
    {
        private readonly IReserveRepository _repository;
        private readonly IReserveMapper _mapper;
        private readonly IReserveNotificationService _notificationService;

        public ReserveService(
            IReserveMapper mapper,
            IReserveRepository repository,
            IReserveNotificationService notificationService)
        {
            _mapper = mapper;
            _repository = repository;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<ResponseReserveDTO>> GetAll()
        {
            var data = await _repository.GetAllAsync();
            return _mapper.ToDTO(data);
        }

        public async Task<ResponseReserveDTO> GetById(Guid id)
        {
            ValidateId(id);

            var data = await _repository.GetByIdAsync(id);

            if (data == null)
                throw new NotFoundException("Reserva", id);  

            return _mapper.ToDTO(data);
        }

        public async Task<IEnumerable<ResponseReserveDTO>> GetByClubId(Guid id)
        {
            ValidateId(id);

            var data = await _repository.GetAllByClubIdAsync(id);

            if (data == null)
                throw new NotFoundException("Reserva", id);

            return _mapper.ToDTO(data);
        }

        public async Task<PagedResultDTO<ResponseReserveDetailDTO>> GetDetailedByClubId(Guid clubId, ReserveQueryDTO query)
        {
            ValidateId(clubId);

            var (items, total) = await _repository.GetAllDetailedByClubIdAsync(clubId, query);

            var itemsDto = items.Select(r => new ResponseReserveDetailDTO
            {
                Id = r.Id,
                Date = r.Date,
                Status = r.Status,
                Name = r.Player.User.Name,
                PhoneNumber = r.Player.User.PhoneNumber,
                UserId = r.Player.UserId,
                DateOfReservation = r.CreatedAt,
                Schedule = new ScheduleReserveDTO
                {
                    StartTime = r.Schedule.StartTime,
                    EndTime = r.Schedule.EndTime,
                    Court = new CourtReserveDTO
                    {
                        Name = r.Schedule.Court.Name,
                        PricePerHour = r.Schedule.Court.PricePerHour,
                        Type = r.Schedule.Court.Type
                    }
                }
            });
            return new PagedResultDTO<ResponseReserveDetailDTO>
            {
                Data = itemsDto,
                TotalCount = total,
                Page = query.Page,
                PageSize = query.PageSize
            };
                
        }

        public async Task<PagedResultDTO<ResponseReserveDetailToPlayerDTO>> GetDetailedByPlayerId(Guid playerId, ReserveQueryDTO query)
        {
            ValidateId(playerId);

            var (items, total) = await _repository.GetAllDetailedByPlayerIdAsync(playerId,query);

            var itemsDto = items.Select(r => new ResponseReserveDetailToPlayerDTO
            {
                Id = r.Id,
                Date = r.Date,
                Status = r.Status,
                Club = new ClubReserveDTO()
                {
                    Name = r.Schedule.Court.Club.Name,
                    PhoneNumber = r.Schedule.Court.Club.PhoneNumber
                },
                Schedule = new ScheduleReserveDTO
                {
                    StartTime = r.Schedule.StartTime,
                    EndTime = r.Schedule.EndTime,
                    Court = new CourtReserveDTO
                    {
                        Name = r.Schedule.Court.Name,
                        PricePerHour = r.Schedule.Court.PricePerHour,
                        Type = r.Schedule.Court.Type
                    }
                }
            });
            return new PagedResultDTO<ResponseReserveDetailToPlayerDTO>
            {
                Data = itemsDto,
                TotalCount = total,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }
        
        public async Task<ResponseIdDTO> Create(CreatReserveDTO dto)
        {
            ValidateReserveDTO(dto);

            var entity = new Reserve
            {
                Date = dto.Date,
                Status = StatusEnum.Confirmada,
                PlayerId = dto.PlayerId,
                ScheduleId = dto.ScheduleId,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            // busca com Schedule.Court já carregado pra pegar o ClubId
            var withClub = await _repository.GetByIdWithClubAsync(entity.Id);

            if (withClub != null)
            {
                await _notificationService.NotifyStatusChangedAsync(
                    withClub.Schedule.Court.ClubId,
                    new ReserveAvailabilityChangedDTO
                    {
                        ReserveId = withClub.Id,
                        ScheduleId = withClub.ScheduleId,
                        CourtId = withClub.Schedule.CourtId,
                        Date = withClub.Date,
                        Status = withClub.Status
                    });
            }

            return new ResponseIdDTO { Id = entity.Id };
        }

        public async Task ChangeStatus(Guid id, StatusEnum status)
        {
            ValidateId(id);

            var entity = await _repository.GetByIdWithClubAsync(id);

            if (entity == null)
                throw new NotFoundException("Reserva", id); 

            entity.Status = status;

            _repository.Update(entity);
            await _repository.SaveChangesAsync();

            await _notificationService.NotifyStatusChangedAsync(
                entity.Schedule.Court.ClubId,
                new ReserveAvailabilityChangedDTO
                {
                    ReserveId = entity.Id,
                    ScheduleId = entity.ScheduleId,
                    CourtId = entity.Schedule.CourtId,
                    Date = entity.Date,
                    Status = entity.Status
                });
        }
        
        public async Task<ResponseReserveDTO> Update(Guid id, UpdateReserveDTO dto)
        {
            ValidateId(id);
            ValidateUpdateReserveDTO(dto);

            var data = await _repository.GetByIdAsync(id);

            if (data == null)
                throw new NotFoundException("Reserva", id);

            data.Date = dto.Date;
            data.Status = dto.Status;
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
                throw new NotFoundException("Reserva", id);

            await _repository.DeleteAsync(id);
        }

        private static void ValidateId(Guid id)
        {
            if (id == Guid.Empty)
                throw new ValidationException("O ID informado é inválido.");
        }

        private static void ValidateReserveDTO(CreatReserveDTO dto)
        {
            if (dto == null)
                throw new ValidationException("Os dados da reserva são obrigatórios.");
        }

        private static void ValidateUpdateReserveDTO(UpdateReserveDTO dto)
        {
            if (dto == null)
                throw new ValidationException("Os dados de atualização são obrigatórios.");
        }
    }
}