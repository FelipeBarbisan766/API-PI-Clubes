using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_PI_Clubes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _service;

        public ScheduleController(IScheduleService service)
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

        [HttpGet("court/{courtId}")]
        public async Task<IActionResult> GetByCourtId(Guid courtId, CancellationToken cancellationToken)
        {
            var result = await _service.GetByCourtId(courtId,cancellationToken);
            return Ok(result);
        }
        
        [HttpGet("court/{courtId}/availability")]
        public async Task<IActionResult> GetAvailabilityByCourtAndDate(Guid courtId, [FromQuery] DateOnly date, CancellationToken cancellationToken)
        {
            var result = await _service.GetAvailabilityByCourtAndDate(courtId, date,cancellationToken);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreatScheduleDTO dto, CancellationToken cancellationToken)
        {
            var result = await _service.Create(dto,cancellationToken);
            return Ok(result);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost("court/{courtId}/bulk")]
        public async Task<IActionResult> CreateBulk(Guid courtId, CreateBulkScheduleDTO dto, CancellationToken cancellationToken)
        {
            dto.CourtId = courtId;
            var result = await _service.CreateBulk(dto,cancellationToken);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateScheduleDTO dto, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var result = await _service.Update(userId, id, dto,cancellationToken);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            await _service.Delete(userId, id,cancellationToken);
            return NoContent();
        }
    }
}