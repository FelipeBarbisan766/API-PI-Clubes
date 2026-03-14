using API_PI_Clubes.Data;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.DTOs;
using API_PI_Clubes.Model.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API_PI_Clubes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservaController: ControllerBase
    {
        private readonly AppDbContext _context;
        public ReservaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reservas = await _context.Reservas.Where(c => c.IsActive).ToListAsync();

            return Ok(reservas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var reservas = await _context.Reservas.FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (reservas == null)
                return NotFound();

            return Ok(reservas);
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreatReservaDTO dto)
        {
            var reservas = new Reserva
            {
                Name = dto.Name,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                Date = dto.Date,
                Status = dto.Status,
                HorarioId = dto.HorarioId
            };

            _context.Reservas.Add(reservas);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateReservaDTO dto)
        {
            var reservas = await _context.Reservas.FindAsync(id);

            if (reservas == null)
                return NotFound();

            reservas.Name = dto.Name;
            reservas.PhoneNumber = dto.PhoneNumber;
            reservas.Email = dto.Email;
            reservas.Date = dto.Date;
            reservas.Status = dto.Status;

            await _context.SaveChangesAsync();

            return Ok(reservas);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var reservas = await _context.Reservas.FindAsync(id);

            if (reservas == null)
                return NotFound();

            reservas.IsActive = false;
            reservas.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
