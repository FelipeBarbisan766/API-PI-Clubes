using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_PI_Clubes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourtController : ControllerBase
    {
        private readonly ICourtService _service;
        public CourtController(ICourtService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAll();

            return Ok(result);
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetById(Guid id)
        //{
        //    var data = await _context.Courts.FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

        //    if (data == null)
        //        return NotFound();

        //    return Ok(data);
        //}
        

        //[HttpPost]
        //public async Task<IActionResult> Create(CreatCourtDTO dto)
        //{
        //    var response = new Court
        //    {
        //        Name = dto.Name,
        //        Type = dto.Type,
        //        Surface = dto.Surface,
        //        IsCovered = dto.IsCovered,
        //        PricePerHour = dto.PricePerHour,
        //        Description = dto.Description,
        //        ClubId = dto.ClubId
        //    };

        //    _context.Courts.Add(response);
        //    await _context.SaveChangesAsync();

        //    return Ok();
        //}

        //[HttpPut("{id}")]
        //public async Task<IActionResult> Update(Guid id, UpdateCourtDTO dto)
        //{
        //    var data = await _context.Courts.FindAsync(id);

        //    if (data == null)
        //        return NotFound();

        //    data.Name = dto.Name;
        //    data.Type = dto.Type;
        //    data.Surface = dto.Surface;
        //    data.IsCovered = dto.IsCovered;
        //    data.PricePerHour = dto.PricePerHour;
        //    data.Description = dto.Description;
        //    data.UpdatedAt = DateTime.UtcNow;

        //    await _context.SaveChangesAsync();

        //    return Ok(data);
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(Guid id)
        //{
        //    var data = await _context.Courts.FindAsync(id);

        //    if (data == null)
        //        return NotFound();

        //    data.IsActive = false;
        //    data.UpdatedAt = DateTime.UtcNow;

        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}
    }
}
