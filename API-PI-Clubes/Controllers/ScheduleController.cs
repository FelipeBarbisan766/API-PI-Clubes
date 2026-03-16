using API_PI_Clubes.Data;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ScheduleController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var datas = await _context.Schedules.Where(c => c.IsActive).ToListAsync();

            return Ok(datas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var data = await _context.Schedules.FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (data == null)
                return NotFound();

            return Ok(data);
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreatScheduleDTO dto)
        {
            var response = new Schedule
            {
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                IsBlocked = dto.IsBlocked,
                IsReserved = dto.IsReserved,
                IsFixed = dto.IsFixed,
                DayOfWeek = dto.DayOfWeek,
                CourtId = dto.CourtId
            };

            _context.Schedules.Add(response);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateScheduleDTO dto)
        {
            var data = await _context.Schedules.FindAsync(id);

            if (data == null)
                return NotFound();

            data.StartTime = dto.StartTime;
            data.EndTime = dto.EndTime;
            data.IsBlocked = dto.IsBlocked;
            data.IsReserved = dto.IsReserved;
            data.IsFixed = dto.IsFixed;
            data.DayOfWeek = dto.DayOfWeek;
            data.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var data = await _context.Schedules.FindAsync(id);

            if (data == null)
                return NotFound();

            data.IsActive = false;
            data.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
