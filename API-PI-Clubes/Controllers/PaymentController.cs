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
        private readonly ILogger<PaymentController> _logger;
        public PaymentController(IPaymentService service, ILogger<PaymentController> logger)
        {
            _logger = logger;
            _service = service;
        }
        
        [HttpPost("initiate")]
        public async Task<IActionResult> Initiate([FromBody] CreatePaymentDto dto)
        {
            var result = await _service.InitiateAsync(dto);
            return Ok(result);
        }
     
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook(
            [FromBody] MercadoPagoWebhookDto? bodyDto,
            [FromQuery] string? id,
            [FromQuery] string? topic,
            [FromQuery] string? type)
        {
            try
            {
              
                _logger.LogInformation(
                    "Webhook recebido — body: {@Body} | query id: {Id} | topic: {Topic} | type: {Type}",
                    bodyDto, id, topic, type);
     
                
                var webhookData = new MercadoPagoWebhookDto(
                    Action: bodyDto?.Action ?? topic ?? type ?? string.Empty,
                    Data: new MercadoPagoWebhookDataDto(
                        Id: bodyDto?.Data?.Id ?? id ?? string.Empty
                    )
                );
     
                if (string.IsNullOrEmpty(webhookData.Data.Id))
                {
                    _logger.LogWarning("Webhook recebido sem ID. Ignorando.");
                    return Ok(); 
                }
     
                await _service.HandleWebhookAsync(webhookData);
                return Ok();
            }
            catch (Exception ex)
            {
                
                _logger.LogError(ex, "Erro ao processar webhook do Mercado Pago.");
                return Ok();
            }
        }
     
        [HttpGet("history/{adminId:guid}")]
        public async Task<IActionResult> History(Guid adminId)
        {
            var history = await _service.GetHistoryByAdminAsync(adminId);
            return Ok(history);
        }


    }
}
