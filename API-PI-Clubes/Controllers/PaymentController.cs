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
        public async Task<IActionResult> Initiate([FromBody] CreatePaymentDto dto, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var result = await _service.InitiateAsync(dto, userId, cancellationToken);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook(
            [FromBody] MercadoPagoWebhookDto? bodyDto,
            [FromQuery(Name = "data.id")] string? dataId,
            [FromQuery] string? type)
        {
            _logger.LogInformation(
                "Webhook recebido — body: {@Body} | data.id: {DataId} | type: {Type}",
                bodyDto, dataId, type);

            var id = dataId ?? bodyDto?.Data?.Id;
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Webhook recebido sem ID. Ignorando.");
                return Ok();
            }

            var webhookData = new MercadoPagoWebhookDto(
                Action: bodyDto?.Action,
                Type: bodyDto?.Type ?? type,
                Data: new MercadoPagoWebhookDataDto(id)
            );

            var signatureHeader = Request.Headers["x-signature"].ToString();
            var requestIdHeader = Request.Headers["x-request-id"].ToString();

            await _service.HandleWebhookAsync(webhookData, signatureHeader, requestIdHeader, CancellationToken.None);
            return Ok();
        }

        [HttpPost("{id:guid}/sync")]
        public async Task<IActionResult> Sync(Guid id, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var result = await _service.SyncAsync(id, userId, cancellationToken);
            return Ok(result);
        }

        [HttpGet("history")]
        public async Task<IActionResult> History(CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var history = await _service.GetHistoryByAdminAsync(userId, cancellationToken);
            return Ok(history);
        }
    }
}