using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IMappers;
using API_PI_Clubes.Application.Interfaces.IRepositories;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.Enums;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API_PI_Clubes.Application.Services
{
    public class ReserveService : IReserveService
    {
        private readonly IReserveRepository _repository;
        private readonly IReserveMapper _mapper;

        public ReserveService(IReserveMapper mapper, IReserveRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
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
                throw new InvalidOperationException("Reserve not found");

            return _mapper.ToDTO(data);
        }
        public async Task<ResponseIdDTO> Create(CreatReserveDTO dto)
        {
            ValidateReserveDTO(dto);

            var entity = new Reserve
            {
                Date = dto.Date,
                Status = StatusEnum.AguardandoConfirmacao,
                PlayerId = dto.PlayerId,
                ScheduleId = dto.ScheduleId,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            return new ResponseIdDTO { Id = entity.Id };

        }


        public async Task<ResponseReserveDTO> Update(Guid id, UpdateReserveDTO dto)
        {
            ValidateId(id);
            ValidateUpdateReserveDTO(dto);

            var data = await _repository.GetByIdAsync(id);

            if (data == null)
                throw new InvalidOperationException("Reserve not found");

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
                throw new InvalidOperationException("Reserve not found");

            await _repository.DeleteAsync(id);
        }

        private static void ValidateId(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid ID", nameof(id));
        }

        private static void ValidateReserveDTO(CreatReserveDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
        }

        private static void ValidateUpdateReserveDTO(UpdateReserveDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
        }
    }
}
