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
            var adminId = User.GetUserId();
            var result = await _service.InitiateAsync(dto, adminId, cancellationToken);
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

            // O Mercado Pago assina usando o data.id da QUERY STRING, então ela é a fonte principal.
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

            // Sem try/catch de propósito (padrão do projeto): se algo transitório falhar (API do MP fora,
            // banco), o ExceptionMiddleware devolve erro e o Mercado Pago reenvia a notificação depois.
            // Casos inválidos/ignoráveis o service resolve com return, e aí respondemos 200.
            //
            // CancellationToken.None: quem chama não é o usuário final, e o MP derruba a conexão após 22s.
            // Usar o RequestAborted poderia cancelar o processamento no meio. Troque se decidir diferente.
            await _service.HandleWebhookAsync(webhookData, signatureHeader, requestIdHeader, CancellationToken.None);
            return Ok();
        }

        // Reconcilia o pagamento com o Mercado Pago. Útil para Pix (não há redirecionamento de volta),
        // para webhook atrasado/perdido e para reparar pagamento Confirmed sem assinatura ativada.
        [HttpPost("{id:guid}/sync")]
        public async Task<IActionResult> Sync(Guid id, CancellationToken cancellationToken)
        {
            var adminId = User.GetUserId();
            var result = await _service.SyncAsync(id, adminId, cancellationToken);
            return Ok(result);
        }

        [HttpGet("history")]
        public async Task<IActionResult> History(CancellationToken cancellationToken)
        {
            var adminId = User.GetUserId();
            var history = await _service.GetHistoryByAdminAsync(adminId, cancellationToken);
            return Ok(history);
        }
    }
}