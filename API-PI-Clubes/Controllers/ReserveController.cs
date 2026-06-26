using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_PI_Clubes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReserveController : ControllerBase
    {
        private readonly IReserveService _service;

        public ReserveController(IReserveService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAll();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetById(id);
            return Ok(result);
        }

        [HttpGet("club/{id}")]
        public async Task<IActionResult> GetByClubId(Guid id)
        {
            var result = await _service.GetByClubId(id);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("club/{id}/details")]
        public async Task<IActionResult> GetDetailedByClubId(Guid id)
        {
            var result = await _service.GetDetailedByClubId(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatReserveDTO dto)
        {
            var result = await _service.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateReserveDTO dto)
        {
            var result = await _service.Update(id, dto);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.Delete(id);
            return NoContent();
        }
    }
}