using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;


namespace API_PI_Clubes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController: ControllerBase
    {
        private readonly IPaymentService _service;
        public PaymentController(IPaymentService service)
        {
            _service = service;
        }
        
        [HttpPost("initiate")]
        public async Task<IActionResult> Initiate([FromBody] CreatePaymentDto dto)
        {
            var result = await _service.InitiateAsync(dto);
            return Ok(result); 
        }
 
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] MercadoPagoWebhookDto dto)
        {
            await _service.HandleWebhookAsync(dto);
            return Ok(); 
        }
 
        [HttpGet("history/{adminId:guid}")]
        public async Task<IActionResult> History(Guid adminId)
        {
            var history = await _service.GetHistoryByAdminAsync(adminId);
            return Ok(history);
        }

    }
}
