using API_PI_Clubes.Data;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourtController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CourtController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var datas = await _context.Courts.Where(c => c.IsActive).ToListAsync();

            return Ok(datas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var data = await _context.Courts.FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (data == null)
                return NotFound();

            return Ok(data);
        }
        

        [HttpPost]
        public async Task<IActionResult> Create(CreatCourtDTO dto)
        {
            var response = new Court
            {
                Name = dto.Name,
                Type = dto.Type,
                Surface = dto.Surface,
                IsCovered = dto.IsCovered,
                PricePerHour = dto.PricePerHour,
                Description = dto.Description,
                ClubId = dto.ClubId
            };

            _context.Courts.Add(response);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateCourtDTO dto)
        {
            var data = await _context.Courts.FindAsync(id);

            if (data == null)
                return NotFound();

            data.Name = dto.Name;
            data.Type = dto.Type;
            data.Surface = dto.Surface;
            data.IsCovered = dto.IsCovered;
            data.PricePerHour = dto.PricePerHour;
            data.Description = dto.Description;
            data.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var data = await _context.Courts.FindAsync(id);

            if (data == null)
                return NotFound();

            data.IsActive = false;
            data.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
