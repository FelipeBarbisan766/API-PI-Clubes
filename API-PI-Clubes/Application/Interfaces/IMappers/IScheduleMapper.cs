using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Model;

namespace API_PI_Clubes.Application.Interfaces.IMappers
{
    public interface IScheduleMapper
    {
        ResponseScheduleDTO ToDTO(Schedule schedule);
        IEnumerable<ResponseScheduleDTO> ToDTO(IEnumerable<Schedule> schedules);
    }
}