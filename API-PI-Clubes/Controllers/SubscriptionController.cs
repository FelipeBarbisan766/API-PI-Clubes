using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IServices;
using API_PI_Clubes.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_PI_Clubes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _service;
        private readonly IPlanLimitService _planLimitService;
        public SubscriptionController(
            ISubscriptionService service,
            IPlanLimitService planLimitService
            )
        {
            _service = service;
            _planLimitService = planLimitService;
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var adminId = User.GetUserId();
            var subscription = await _service.GetActiveByAdminAsync(adminId);

            if (subscription is null)
                return NotFound("Nenhuma assinatura ativa encontrada.");

            return Ok(subscription);
        }

        [HttpGet("check-access")]
        public async Task<IActionResult> CheckAccess()
        {
            var adminId = User.GetUserId();
            var hasAccess = await _service.CheckAccessAsync(adminId);
            return Ok(new { hasAccess });
        }

        [HttpPost("cancel/{subscriptionId:guid}")]
        public async Task<IActionResult> Cancel(Guid subscriptionId)
        {
            var userId = User.GetUserId();
            await _service.CancelAsync(subscriptionId, userId);
            return NoContent();
        }
        [HttpGet("me/usage")]
        public async Task<ActionResult<PlanUsageDTO>> GetMyUsage()
        {
            var adminId = User.GetUserId();
            var usage = await _planLimitService.GetUsageSummaryAsync(adminId); 
            return Ok(usage);
        }
    }
}