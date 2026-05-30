using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;


namespace API_PI_Clubes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController: ControllerBase
    {
        private readonly ISubscriptionService _service;
        public SubscriptionController(ISubscriptionService service)
        {
            _service = service;
        }
        
        [HttpGet("active/{adminId:guid}")]
        public async Task<IActionResult> GetActive(Guid adminId)
        {
            var subscription = await _service.GetActiveByAdminAsync(adminId);
 
            if (subscription is null)
                return NotFound("Nenhuma assinatura ativa encontrada.");
 
            return Ok(subscription);
        }
 
        [HttpGet("check-access/{adminId:guid}")]
        public async Task<IActionResult> CheckAccess(Guid adminId)
        {
            var hasAccess = await _service.CheckAccessAsync(adminId);
            return Ok(new { hasAccess });
        }
 
        [HttpPost("cancel/{subscriptionId:guid}")]
        public async Task<IActionResult> Cancel(Guid subscriptionId)
        {
            await _service.CancelAsync(subscriptionId);
            return NoContent();
        }

    }
}
