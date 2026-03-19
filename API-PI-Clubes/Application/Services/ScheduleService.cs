using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Model;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace API_PI_Clubes.Application.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly AppDbContext _context;
        public ScheduleService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ResponseScheduleDTO>> GetAll()
        {
            return await _context.Schedules
                .Where(c => c.IsActive)
                .Select(c => new ResponseScheduleDTO
                {
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    IsBlocked = c.IsBlocked,
                    IsReserved = c.IsReserved,
                    IsFixed = c.IsFixed,
                    DayOfWeek = c.DayOfWeek,
                    CourtId = c.CourtId
                })
                .ToListAsync();
        }
        public async Task<ResponseScheduleDTO> GetById(Guid id)
        {
            var data = await _context.Schedules
                .Where(c => c.Id == id && c.IsActive)
                .Select(c => new ResponseScheduleDTO
                {
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    IsBlocked = c.IsBlocked,
                    IsReserved = c.IsReserved,
                    IsFixed = c.IsFixed,
                    DayOfWeek = c.DayOfWeek,
                    CourtId = c.CourtId
                })
                .FirstOrDefaultAsync();

            if (data == null)
                throw new Exception("Schedule not found");

            return data;
        }

        public async Task<ResponseScheduleDTO> Create(CreatScheduleDTO dto)
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

            _context.Schedules.Add(entity);
            await _context.SaveChangesAsync();

            return new ResponseScheduleDTO
            {
                StartTime = entity.StartTime,
                EndTime= entity.EndTime,
                IsBlocked= entity.IsBlocked,
                IsReserved = entity.IsReserved,
                IsFixed = entity.IsFixed,
                DayOfWeek = entity.DayOfWeek,
            };
        }

        public async Task<ResponseScheduleDTO> Update(Guid id, UpdateScheduleDTO dto)
        {
            var data = await _context.Schedules.FirstOrDefaultAsync(c => c.Id == id);

            if (data == null)
                throw new Exception("Schedule not found");

            data.StartTime = dto.StartTime;
            data.EndTime = dto.EndTime;
            data.IsBlocked = dto.IsBlocked;
            data.IsReserved = dto.IsReserved;
            data.IsFixed = dto.IsFixed;
            data.DayOfWeek = dto.DayOfWeek;
            data.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ResponseScheduleDTO
            {
                StartTime  =   data.StartTime,
                EndTime    =   data.EndTime,
                IsBlocked  =   data.IsBlocked,
                IsReserved =   data.IsReserved,
                IsFixed    =   data.IsFixed,
                DayOfWeek  =   data.DayOfWeek,
                CourtId    =   data.CourtId
            };
        }

        public async Task Delete(Guid id)
        {
            var data = await _context.Schedules.FirstOrDefaultAsync(c => c.Id == id);

            if (data == null)
                throw new Exception("Schedule not found");

            data.IsActive = false;
            data.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
