using API_PI_Clubes.Data;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Controllers
{
    [Route("api/[controller]")] 
    [ApiController]
    public class ClubController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ClubController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var datas = await _context.Clubs
                .Where(c => c.IsActive)
                .ToListAsync();

            return Ok(datas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var data = await _context.Clubs
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (data == null)
                return NotFound();

            return Ok(data);
        }
        //-------------------------
        [HttpGet("/test/{id}")]
        public async Task<IActionResult> GetByIdAndInclude(Guid id)
        {
            // 1. Busca no banco incluindo as quadras
            var data = await _context.Clubs
                .Include(c => c.Courts)
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (data == null)
                return NotFound();

            // 2. Mapeia da Entidade para o DTO
            var response = new ResponseClubDTO
            {
                Id = data.Id,
                Name = data.Name,
                PhoneNumber = data.PhoneNumber,
                Description = data.Description,
                Courts = data.Courts.Select(q => new ResponseCourtDTO
                {
                    Id = q.Id,
                    Name = q.Name,
                    Type = q.Type,
                    Surface = q.Surface,
                    IsCovered = q.IsCovered,
                    PricePerHour = q.PricePerHour
                }).ToList()
            };

            return Ok(response);
        }
        //-------------------------

        [HttpPost]
        public async Task<IActionResult> Create(CreateClubDTO dto)
        {
            var response = new Club
            {
                Name = dto.Name,
                PhoneNumber = dto.PhoneNumber,
                Description = dto.Description
            };

            _context.Clubs.Add(response);
            await _context.SaveChangesAsync();

            return Ok();
        }
        //-------------------------
        [HttpPost("{clubId}/admins/{adminId}")]
        public async Task<IActionResult> AddAdminToClub(Guid clubId, Guid adminId)
        {
            var exists = await _context.ClubAdmins
                .AnyAsync(ca => ca.ClubId == clubId && ca.AdminId == adminId);

            if (exists)
                return BadRequest("Admin already linked to this club");

            var clubAdmin = new ClubAdmin
            {
                ClubId = clubId,
                AdminId = adminId
            };

            _context.ClubAdmins.Add(clubAdmin);
            await _context.SaveChangesAsync();

            return Ok();
        }
        //-------------------------

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateClubDTO dto)
        {
            var data = await _context.Clubs.FindAsync(id);

            if (data == null)
                return NotFound();

            data.Name = dto.Name;
            data.PhoneNumber = dto.PhoneNumber;
            data.Description = dto.Description;
            data.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var data = await _context.Clubs.FindAsync(id);

            if (data == null)
                return NotFound();

            data.IsActive = false;
            data.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        //-------------------------
        [HttpDelete("{clubId}/admins/{adminId}")]
        public async Task<IActionResult> RemoveAdmin(Guid clubId, Guid adminId)
        {
            var clubAdmin = await _context.ClubAdmins
                .FirstOrDefaultAsync(ca => ca.ClubId == clubId && ca.AdminId == adminId);

            if (clubAdmin == null)
                return NotFound();

            _context.ClubAdmins.Remove(clubAdmin);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        //-------------------------
    }

}
