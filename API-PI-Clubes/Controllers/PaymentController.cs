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
    public class PaymentController : ControllerBase
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
            var adminId = User.GetUserId();
            var result = await _service.InitiateAsync(dto, adminId);
            return Ok(result);
        }

        [AllowAnonymous]
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

                var signatureHeader = Request.Headers["x-signature"].ToString();
                var requestIdHeader = Request.Headers["x-request-id"].ToString();

                await _service.HandleWebhookAsync(webhookData, signatureHeader, requestIdHeader);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar webhook do Mercado Pago.");
                return Ok();
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> History()
        {
            var adminId = User.GetUserId();
            var history = await _service.GetHistoryByAdminAsync(adminId);
            return Ok(history);
        }
    }
}