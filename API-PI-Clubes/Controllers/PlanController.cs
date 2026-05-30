using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;


namespace API_PI_Clubes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanController: ControllerBase
    {
        private readonly IPlanService _service;
        public PlanController(IPlanService service)
        {
            _service = service;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var plans = await _service.GetAllActiveAsync();
            return Ok(plans);
        }
 
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePlanDto dto)
        {
            var plan = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetAll), new { id = plan.Id }, plan);
        }
 
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePlanDto dto)
        {
            var plan = await _service.UpdateAsync(id, dto);
            return Ok(plan);
        }
 
        [HttpPatch("{id:guid}/set-active")]
        public async Task<IActionResult> SetActive(Guid id, [FromQuery] bool isActive)
        {
            await _service.SetActiveAsync(id, isActive);
            return NoContent();
        }

    }
}
