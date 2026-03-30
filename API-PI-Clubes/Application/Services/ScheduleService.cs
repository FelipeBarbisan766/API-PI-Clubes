using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IMappers;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Model;

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

        public async Task<ResponseIdDTO> Create(CreatScheduleDTO dto)
        {
            ValidateScheduleDTO(dto);

            var entity = new Schedule
            {
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                IsBlocked = dto.IsBlocked,
                IsReserved = dto.IsReserved,
                IsFixed = dto.IsFixed,
                DayOfWeek = dto.DayOfWeek,
                CourtId = dto.CourtId
            };

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            return new ResponseIdDTO { Id = entity.Id };
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
            data.IsBlocked = dto.IsBlocked;
            data.IsReserved = dto.IsReserved;
            data.IsFixed = dto.IsFixed;
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
    }
}