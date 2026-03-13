using API_PI_Clubes.Data;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuadraController : ControllerBase
    {
        private readonly AppDbContext _context;
        public QuadraController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var quadras = await _context.Quadras.Where(c => c.IsActive).ToListAsync();

            return Ok(quadras);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var quadras = await _context.Quadras.FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (quadras == null)
                return NotFound();

            return Ok(quadras);
        }
        

        [HttpPost]
        public async Task<IActionResult> Create(CreatQuadraDTO dto)
        {
            var quadras = new Quadra
            {
                Name = dto.Name,
                Type = dto.Type,
                Surface = dto.Surface,
                IsCovered = dto.IsCovered,
                PricePerHour = dto.PricePerHour,
                Description = dto.Description,
                ClubeId = dto.ClubeId
            };

            _context.Quadras.Add(quadras);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateQuadraDTO dto)
        {
            var quadras = await _context.Quadras.FindAsync(id);

            if (quadras == null)
                return NotFound();

            quadras.Name = dto.Name;
            quadras.Type = dto.Type;
            quadras.Surface = dto.Surface;
            quadras.IsCovered = dto.IsCovered;
            quadras.PricePerHour = dto.PricePerHour;
            quadras.Description = dto.Description;
            quadras.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(quadras);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var quadras = await _context.Quadras.FindAsync(id);

            if (quadras == null)
                return NotFound();

            quadras.IsActive = false;
            quadras.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
