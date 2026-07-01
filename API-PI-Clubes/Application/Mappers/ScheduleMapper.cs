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
                State =  schedule.State,
                DayOfWeek = schedule.DayOfWeek,
                CourtId = schedule.CourtId
            };
        }

        public IEnumerable<ResponseScheduleDTO> ToDTO(IEnumerable<Schedule> schedules)
        {
            return schedules.Select(ToDTO);
        }
        public IEnumerable<ResponseScheduleAvailabilityDTO> ToAvailabilityDTO(IEnumerable<Schedule> schedules)
        {
            return schedules.Select(s =>
            {
                // O Include já filtrou as Reserves pela data e pelo IsActive,
                // então qualquer item aqui é uma reserva real para aquele dia.
                var reserve = s.Reserves?.FirstOrDefault();
 
                return new ResponseScheduleAvailabilityDTO
                {
                    Id         = s.Id,
                    StartTime  = s.StartTime,
                    EndTime    = s.EndTime,
                    State      = s.State,
                    DayOfWeek  = s.DayOfWeek,
 
                    Reserve = reserve == null ? null : new ResponseReserveInfoDTO
                    {
                        Id       = reserve.Id,
                        Date     = reserve.Date,
                        Status   = reserve.Status,
                        PlayerId = reserve.PlayerId
                    }
                };
            });
        }

    }
}