using API_PI_Clubes.Data;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Controllers
{
    [Route("api/[controller]")] 
    [ApiController]
    public class ClubeController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ClubeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var clubes = await _context.Clubes
                .Where(c => c.IsActive)
                .ToListAsync();

            return Ok(clubes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var clube = await _context.Clubes
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (clube == null)
                return NotFound();

            return Ok(clube);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateClubeDTO dto)
        {
            var clube = new Clube
            {
                Name = dto.Name,
                PhoneNumber = dto.PhoneNumber,
                Description = dto.Description
            };

            _context.Clubes.Add(clube);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateClubeDTO dto)
        {
            var clube = await _context.Clubes.FindAsync(id);

            if (clube == null)
                return NotFound();

            clube.Name = dto.Name;
            clube.PhoneNumber = dto.PhoneNumber;
            clube.Description = dto.Description;
            clube.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(clube);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var clube = await _context.Clubes.FindAsync(id);

            if (clube == null)
                return NotFound();

            clube.IsActive = false;
            clube.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
    
}
