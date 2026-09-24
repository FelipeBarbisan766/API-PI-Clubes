using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API_PI_Clubes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class PlanController: ControllerBase
    {
        private readonly IPlanService _service;
        public PlanController(IPlanService service)
        {
            _service = service;
        }
        
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var plans = await _service.GetAllActiveAsync(cancellationToken);
            return Ok(plans);
        }
 
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePlanDto dto,CancellationToken cancellationToken)
        {
            var plan = await _service.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetAll), new { id = plan.Id }, plan);
        }
 
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePlanDto dto, CancellationToken cancellationToken)
        {
            var plan = await _service.UpdateAsync(id, dto, cancellationToken);
            return Ok(plan);
        }
 
        [HttpPatch("{id:guid}/set-active")]
        public async Task<IActionResult> SetActive(Guid id, [FromQuery] bool isActive, CancellationToken cancellationToken)
        {
            await _service.SetActiveAsync(id, isActive, cancellationToken);
            return NoContent();
        }

    }
}
