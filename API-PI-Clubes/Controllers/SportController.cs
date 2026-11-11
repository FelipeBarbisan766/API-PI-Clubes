using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_PI_Clubes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SportController : ControllerBase
    {
        private readonly ISportService _service;

        public SportController(ISportService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _service.GetAll(cancellationToken);
            return Ok(result);
        }
        
        [HttpGet("Backoffice")]
        [Authorize(Policy = BackofficeAuth.Policy)]
        public async Task<IActionResult> GetAll([FromQuery] bool includeInactive, CancellationToken cancellationToken)
        {
            var result = await _service.GetAllForBackoffice(includeInactive, cancellationToken);
            return Ok(result);
        }

        [HttpPost("Backoffice")]
        [Authorize(Policy = BackofficeAuth.Policy)]
        public async Task<IActionResult> Create([FromBody] SaveSportDTO dto, CancellationToken cancellationToken)
        {
            var result = await _service.Create(dto, cancellationToken);
            return Ok(result);
        }

        [HttpPut("Backoffice/{id:guid}")]
        [Authorize(Policy = BackofficeAuth.Policy)]
        public async Task<IActionResult> Update(Guid id, [FromBody] SaveSportDTO dto, CancellationToken cancellationToken)
        {
            var result = await _service.Update(id, dto, cancellationToken);
            return Ok(result);
        }

        [HttpPatch("Backoffice/{id:guid}/deactivate")]
        [Authorize(Policy = BackofficeAuth.Policy)]
        public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
        {
            await _service.SetActive(id, false, cancellationToken);
            return NoContent();
        }

        [HttpPatch("Backoffice/{id:guid}/activate")]
        [Authorize(Policy = BackofficeAuth.Policy)]
        public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
        {
            await _service.SetActive(id, true, cancellationToken);
            return NoContent();
        }
    }
}