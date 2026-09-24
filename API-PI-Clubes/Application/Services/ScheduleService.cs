using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Exceptions;
using API_PI_Clubes.Application.Interfaces.IMappers;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.Enums;

namespace API_PI_Clubes.Application.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _repository;
        private readonly IScheduleMapper _mapper;

        public ScheduleService(IScheduleRepository repository, IScheduleMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ResponseScheduleDTO>> GetAll(CancellationToken cancellationToken)
        {
            var data = await _repository.GetAllAsync(cancellationToken);
            return _mapper.ToDTO(data);
        }

        public async Task<ResponseScheduleDTO> GetById(Guid id,CancellationToken cancellationToken)
        {
            ValidateId(id);

            var data = await _repository.GetByIdAsync(id,cancellationToken);

            if (data == null)
                throw new NotFoundException("Horário", id); 

            return _mapper.ToDTO(data);
        }

        public async Task<IEnumerable<ResponseScheduleDTO>> GetByCourtId(Guid courtId, CancellationToken cancellationToken  )
        {
            ValidateId(courtId);

            var data = await _repository.GetByCourtIdAsync(courtId,cancellationToken);
            return _mapper.ToDTO(data);
        }
        public async Task<IEnumerable<ResponseScheduleAvailabilityDTO>> GetAvailabilityByCourtAndDate(
            Guid courtId, DateOnly date, CancellationToken cancellationToken)
        {
            ValidateId(courtId);
 
            if (date == DateOnly.MinValue)
                throw new ValidationException("Data inválida.");
 
            var schedules = await _repository.GetByCourtAndDateAsync(courtId, date,cancellationToken);
 
            return _mapper.ToAvailabilityDTO(schedules);
        }

        public async Task<ResponseIdDTO> Create(CreatScheduleDTO dto, CancellationToken cancellationToken)
        {
            ValidateScheduleDTO(dto);

            var entity = new Schedule
            {
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                State = StateEnum.Actived,
                DayOfWeek = dto.DayOfWeek,
                CourtId = dto.CourtId
            };

            await _repository.AddAsync(entity,cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return new ResponseIdDTO { Id = entity.Id };
        }
        
public async Task<ResponseBulkScheduleDTO> CreateBulk(CreateBulkScheduleDTO dto, CancellationToken cancellationToken)
{
    ValidateBulkScheduleDTO(dto);

    var existing = await _repository.GetByCourtAndDaysOfWeekAsync(dto.CourtId, dto.DaysOfWeek,cancellationToken);

    var toCreate = new List<Schedule>();
    var conflicts = new List<ScheduleConflictDTO>();

    foreach (var day in dto.DaysOfWeek)
    {
        var current = dto.StartTime;

        while (current.Add(TimeSpan.FromMinutes(dto.SlotDurationMinutes)) <= dto.EndTime)
        {
            var slotEnd = current.Add(TimeSpan.FromMinutes(dto.SlotDurationMinutes));

            var hasOverlap = existing.Any(s =>
                s.DayOfWeek == day &&
                current < s.EndTime &&
                slotEnd > s.StartTime);

            var hasOverlapWithBatch = toCreate.Any(s =>
                s.DayOfWeek == day &&
                current < s.EndTime &&
                slotEnd > s.StartTime);

            if (hasOverlap || hasOverlapWithBatch)
            {
                conflicts.Add(new ScheduleConflictDTO
                {
                    DayOfWeek = day,
                    StartTime = current,
                    EndTime = slotEnd,
                    Reason = "Conflito com horário já existente"
                });
            }
            else
            {
                toCreate.Add(new Schedule
                {
                    StartTime = current,
                    EndTime = slotEnd,
                    State = StateEnum.Actived,
                    DayOfWeek = day,
                    CourtId = dto.CourtId
                });
            }

            current = slotEnd;
        }
    }

    if (toCreate.Count > 0)
    {
        await _repository.AddRangeAsync(toCreate,cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken); 
    }

    return new ResponseBulkScheduleDTO
    {
        Created = _mapper.ToDTO(toCreate).ToList(),
        Conflicts = conflicts
    };
}

        public async Task<ResponseScheduleDTO> Update(Guid userId, Guid id, UpdateScheduleDTO dto, CancellationToken cancellationToken)
        {
            ValidateId(id);
            ValidateUpdateScheduleDTO(dto);
            await AuthorizeOwnership(userId, id,cancellationToken);

            var data = await _repository.GetByIdAsync(id,cancellationToken);

            if (data == null)
                throw new NotFoundException("Horário", id);

            data.StartTime = dto.StartTime;
            data.EndTime = dto.EndTime;
            data.State = dto.State;
            data.DayOfWeek = dto.DayOfWeek;
            data.UpdatedAt = DateTime.UtcNow;

            _repository.Update(data);
            await _repository.SaveChangesAsync(cancellationToken);

            return _mapper.ToDTO(data);
        }

        public async Task Delete(Guid userId, Guid id, CancellationToken cancellationToken)
        {
            ValidateId(id);
            await AuthorizeOwnership(userId, id,cancellationToken);

            var exists = await _repository.ExistsAsync(id,cancellationToken);

            if (!exists)
                throw new NotFoundException("Horário", id);

            await _repository.DeleteAsync(id,cancellationToken);
        }

        private async Task AuthorizeOwnership(Guid userId, Guid id, CancellationToken cancellationToken)
        {
            var isOwner = await _repository.IsOwnedByUserAsync(id, userId,cancellationToken);
            if (!isOwner)
                throw new ForbiddenException("Você não tem permissão para gerenciar este horário.");
        }

        private static void ValidateId(Guid id)
        {
            if (id == Guid.Empty)
                throw new ValidationException("O ID informado é inválido.");
        }

        private static void ValidateScheduleDTO(CreatScheduleDTO dto)
        {
            if (dto == null)
                throw new ValidationException("Os dados do horário são obrigatórios.");
            if (dto.StartTime >= dto.EndTime)
                throw new ValidationException("O horário de início deve ser antes do de término.");
            if (dto.CourtId == Guid.Empty)
                throw new ValidationException("Quadra inválida.");
        }

        private static void ValidateUpdateScheduleDTO(UpdateScheduleDTO dto)
        {
            if (dto == null)
                throw new ValidationException("Os dados de atualização são obrigatórios.");
            if (dto.StartTime >= dto.EndTime)
                throw new ValidationException("O horário de início deve ser antes do de término.");
        }

        private static void ValidateBulkScheduleDTO(CreateBulkScheduleDTO dto)
        {
            if (dto == null)
                throw new ValidationException("Os dados são obrigatórios.");
            if (dto.CourtId == Guid.Empty)
                throw new ValidationException("Quadra inválida.");
            if (dto.DaysOfWeek == null || dto.DaysOfWeek.Count == 0)
                throw new ValidationException("Informe ao menos um dia da semana.");
            if (dto.StartTime >= dto.EndTime)
                throw new ValidationException("O horário de início deve ser antes do de término.");
            if (dto.SlotDurationMinutes <= 0)
                throw new ValidationException("A duração do slot deve ser maior que zero.");
        }
    }
}
