using API_PI_Clubes.Data;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HorarioController : ControllerBase
    {
        private readonly AppDbContext _context;
        public HorarioController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var horarios = await _context.Horarios.Where(c => c.IsActive).ToListAsync();

            return Ok(horarios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var horarios = await _context.Horarios.FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (horarios == null)
                return NotFound();

            return Ok(horarios);
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreatHorarioDTO dto)
        {
            var horarios = new Horario
            {
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                IsBlocked = dto.IsBlocked,
                IsReserved = dto.IsReserved,
                IsFixed = dto.IsFixed,
                DayOfWeek = dto.DayOfWeek,
                QuadraId = dto.QuadraId
            };

            _context.Horarios.Add(horarios);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateHorarioDTO dto)
        {
            var horarios = await _context.Horarios.FindAsync(id);

            if (horarios == null)
                return NotFound();

            horarios.StartTime = dto.StartTime;
            horarios.EndTime = dto.EndTime;
            horarios.IsBlocked = dto.IsBlocked;
            horarios.IsReserved = dto.IsReserved;
            horarios.IsFixed = dto.IsFixed;
            horarios.DayOfWeek = dto.DayOfWeek;
            horarios.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(horarios);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var horarios = await _context.Horarios.FindAsync(id);

            if (horarios == null)
                return NotFound();

            horarios.IsActive = false;
            horarios.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
