using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API_PI_Clubes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class PlanController : ControllerBase
    {
        private readonly IPlanService _service;
        private readonly IPlanPricingCalculator _calculator;

        public PlanController(IPlanService service,
            IPlanPricingCalculator calculator)
        {
            _service = service;
            _calculator = calculator;
        }

        [HttpPost("custom/calculate")]
        [AllowAnonymous]
        public async Task<ActionResult<CustomPlanPriceDTO>> Calculate(CustomPlanRequestDTO req, CancellationToken ct)
        {
            var response = new CustomPlanPriceDTO(await _calculator.CalculateAsync(req, ct));
            return Ok(response);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var plans = await _service.GetAllActiveAsync(cancellationToken);
            return Ok(plans);
        }

        [HttpGet("custom")]
        public async Task<ActionResult<IEnumerable<PlanResponseDto>>> GetMyCustom(CancellationToken ct)
        {
            var userId = User.GetUserId();
            var response = await _service.GetMyCustomAsync(userId, ct);
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePlanDto dto, CancellationToken cancellationToken)
        {
            var plan = await _service.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetAll), new { id = plan.Id }, plan);
        }

        [HttpPost("custom")]
        public async Task<ActionResult<PlanResponseDto>> CreateCustom(CustomPlanRequestDTO req, CancellationToken ct)
        {
            var userId = User.GetUserId();
            var plan = await _service.CreateCustomAsync(userId, req, ct);
            return Ok(plan);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePlanDto dto,
            CancellationToken cancellationToken)
        {
            var plan = await _service.UpdateAsync(id, dto, cancellationToken);
            return Ok(plan);
        }

        [HttpPatch("{id:guid}/set-active")]
        public async Task<IActionResult> SetActive(Guid id, [FromQuery] bool isActive,
            CancellationToken cancellationToken)
        {
            await _service.SetActiveAsync(id, isActive, cancellationToken);
            return NoContent();
        }
    }
}