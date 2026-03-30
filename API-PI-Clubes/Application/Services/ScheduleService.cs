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
            var data = await _repository.GetByIdAsync(id);

            if (data == null)
                throw new Exception("Schedule not found");

            return _mapper.ToDTO(data);
        }

        public async Task<IEnumerable<ResponseScheduleDTO>> GetByCourtId(Guid courtId)
        {
            var data = await _repository.GetByCourtIdAsync(courtId);
            return _mapper.ToDTO(data);
        }

        public async Task<ResponseIdDTO> Create(CreatScheduleDTO dto)
        {
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
            var data = await _repository.GetByIdAsync(id);

            if (data == null)
                throw new Exception("Schedule not found");

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
            var data = await _repository.GetByIdAsync(id);

            if (data == null)
                throw new Exception("Schedule not found");

            data.IsActive = false;
            data.UpdatedAt = DateTime.UtcNow;

            _repository.Update(data);
            await _repository.SaveChangesAsync();
        }
    }
}