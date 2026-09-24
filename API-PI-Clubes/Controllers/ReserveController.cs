using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Model.Enums;
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
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _service.GetAll(cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _service.GetById(id,cancellationToken);
            return Ok(result);
        }

        [HttpGet("club/{id}")]
        public async Task<IActionResult> GetByClubId(Guid id, CancellationToken cancellationToken)
        {
            var result = await _service.GetByClubId(id,cancellationToken);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("club/{id}/details")]
        public async Task<IActionResult> GetDetailedByClubId(Guid id, [FromQuery] ReserveQueryDTO query, CancellationToken cancellationToken)
        {
            var result = await _service.GetDetailedByClubId(id, query,cancellationToken);
            return Ok(result);
        }
        
        [Authorize]
        [HttpGet("player/{id}/details")]
        public async Task<IActionResult> GetDetailedByPlayerId(Guid id, [FromQuery] ReserveQueryDTO query, CancellationToken cancellationToken)
        {
            var result = await _service.GetDetailedByPlayerId(id, query,cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatReserveDTO dto, CancellationToken cancellationToken)
        {
            var result = await _service.Create(dto,cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPut("status/{id}")]
        public async Task<IActionResult> UpdateStatus(Guid id, StatusEnum status, CancellationToken cancellationToken)
        {
            await _service.ChangeStatus(id, status,cancellationToken);
            return Ok();
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateReserveDTO dto, CancellationToken cancellationToken)
        {
            var result = await _service.Update(id, dto,cancellationToken);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _service.Delete(id,cancellationToken);
            return NoContent();
        }
    }
}