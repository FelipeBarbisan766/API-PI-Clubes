using API_PI_Clubes.Application.DTOs;
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

        public async Task<IEnumerable<ResponseScheduleDTO>> GetAll()
        {
            var data = await _repository.GetAllAsync();
            return _mapper.ToDTO(data);
        }

        public async Task<ResponseScheduleDTO> GetById(Guid id)
        {
            ValidateId(id);

            var data = await _repository.GetByIdAsync(id);

            if (data == null)
                throw new InvalidOperationException("Schedule not found");

            return _mapper.ToDTO(data);
        }

        public async Task<IEnumerable<ResponseScheduleDTO>> GetByCourtId(Guid courtId)
        {
            ValidateId(courtId);

            var data = await _repository.GetByCourtIdAsync(courtId);
            return _mapper.ToDTO(data);
        }
        public async Task<IEnumerable<ResponseScheduleAvailabilityDTO>> GetAvailabilityByCourtAndDate(
            Guid courtId, DateOnly date)
        {
            ValidateId(courtId);
 
            if (date == DateOnly.MinValue)
                throw new ArgumentException("Invalid date", nameof(date));
 
            var schedules = await _repository.GetByCourtAndDateAsync(courtId, date);
 
            return _mapper.ToAvailabilityDTO(schedules);
        }

        public async Task<ResponseIdDTO> Create(CreatScheduleDTO dto)
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

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            return new ResponseIdDTO { Id = entity.Id };
        }
        
public async Task<ResponseBulkScheduleDTO> CreateBulk(CreateBulkScheduleDTO dto)
{
    ValidateBulkScheduleDTO(dto);

    var existing = await _repository.GetByCourtAndDaysOfWeekAsync(dto.CourtId, dto.DaysOfWeek);

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
        await _repository.AddRangeAsync(toCreate);
        await _repository.SaveChangesAsync(); 
    }

    return new ResponseBulkScheduleDTO
    {
        Created = _mapper.ToDTO(toCreate).ToList(),
        Conflicts = conflicts
    };
}

        public async Task<ResponseScheduleDTO> Update(Guid id, UpdateScheduleDTO dto)
        {
            ValidateId(id);
            ValidateUpdateScheduleDTO(dto);

            var data = await _repository.GetByIdAsync(id);

            if (data == null)
                throw new InvalidOperationException("Schedule not found");

            data.StartTime = dto.StartTime;
            data.EndTime = dto.EndTime;
            data.State = dto.State;
            data.DayOfWeek = dto.DayOfWeek;
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
                throw new InvalidOperationException("Schedule not found");

            await _repository.DeleteAsync(id);
        }

        
        private static void ValidateId(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid ID", nameof(id));
        }

        private static void ValidateScheduleDTO(CreatScheduleDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.StartTime >= dto.EndTime)
                throw new ArgumentException("StartTime must be before EndTime");

            if (dto.CourtId == Guid.Empty)
                throw new ArgumentException("Invalid CourtId", nameof(dto.CourtId));
        }

        private static void ValidateUpdateScheduleDTO(UpdateScheduleDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.StartTime >= dto.EndTime)
                throw new ArgumentException("StartTime must be before EndTime");
        }

        private static void ValidateBulkScheduleDTO(CreateBulkScheduleDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.CourtId == Guid.Empty)
                throw new ArgumentException("Invalid CourtId", nameof(dto.CourtId));

            if (dto.DaysOfWeek == null || dto.DaysOfWeek.Count == 0)
                throw new ArgumentException("Informe ao menos um dia da semana");

            if (dto.StartTime >= dto.EndTime)
                throw new ArgumentException("StartTime must be before EndTime");

            if (dto.SlotDurationMinutes <= 0)
                throw new ArgumentException("SlotDurationMinutes deve ser maior que zero");
        }
    }
}
