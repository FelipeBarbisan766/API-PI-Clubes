using API_PI_Clubes.Data;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.DTOs;
using API_PI_Clubes.Model.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using System.Net.NetworkInformation;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API_PI_Clubes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReserveController: ControllerBase
    {
        private readonly AppDbContext _context;
        public ReserveController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var datas = await _context.Reserves.Where(c => c.IsActive).ToListAsync();

            return Ok(datas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var data = await _context.Reserves.FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (data == null)
                return NotFound();

            return Ok(data);
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreatReserveDTO dto)
        {
            var response = new Reserve
            {
                //Name = dto.Name,
                //PhoneNumber = dto.PhoneNumber,
                //Email = dto.Email,
                Date = dto.Date,
                Status = dto.Status,
                ScheduleId = dto.ScheduleId
            };

            _context.Reserves.Add(response);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateReserveDTO dto)
        {
            var data = await _context.Reserves.FindAsync(id);

            if (data == null)
                return NotFound();

            //reserves.Name = dto.Name;
            //reserves.PhoneNumber = dto.PhoneNumber;
            //reserves.Email = dto.Email;
            data.Date = dto.Date;
            data.Status = dto.Status;

            await _context.SaveChangesAsync();

            return Ok(data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var data = await _context.Reserves.FindAsync(id);

            if (data == null)
                return NotFound();

            data.IsActive = false;
            data.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
