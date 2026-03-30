using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IMappers;
using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Mappers
{
    public class ScheduleMapper : IScheduleMapper
    {
        public ResponseScheduleDTO ToDTO(Schedule schedule)
        {
            return new ResponseScheduleDTO
            {
                Id = schedule.Id,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                IsBlocked = schedule.IsBlocked,
                IsReserved = schedule.IsReserved,
                IsFixed = schedule.IsFixed,
                DayOfWeek = schedule.DayOfWeek,
                CourtId = schedule.CourtId
            };
        }

        public IEnumerable<ResponseScheduleDTO> ToDTO(IEnumerable<Schedule> schedules)
        {
            return schedules.Select(ToDTO);
        }
    }
}