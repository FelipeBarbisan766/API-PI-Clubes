using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using API_PI_Clubes.Application.Interfaces.IServices;

namespace API_PI_Clubes.Infrastructure.Services
{
    public class MercadoPagoOrdersClient : IMercadoPagoOrdersClient
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        private readonly HttpClient _http;

        // BaseAddress e Authorization são configurados no AddHttpClient (veja o Program.cs).
        public MercadoPagoOrdersClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<MpOrderResponse> CreateAsync(MpCreateOrderRequest request, string idempotencyKey,
            CancellationToken cancellationToken)
        {
            using var message = new HttpRequestMessage(HttpMethod.Post, "v1/orders")
            {
                Content = JsonContent.Create(request, options: JsonOptions)
            };
            message.Headers.Add("X-Idempotency-Key", idempotencyKey);

            using var response = await _http.SendAsync(message, cancellationToken);
            await EnsureSuccessAsync(response, cancellationToken);

            return await response.Content.ReadFromJsonAsync<MpOrderResponse>(JsonOptions, cancellationToken)
                   ?? throw new InvalidOperationException("Resposta vazia ao criar order no Mercado Pago.");
        }

        public async Task<MpOrderResponse> GetAsync(string orderId, CancellationToken cancellationToken)
        {
            using var response = await _http.GetAsync($"v1/orders/{Uri.EscapeDataString(orderId)}", cancellationToken);
            await EnsureSuccessAsync(response, cancellationToken);

            return await response.Content.ReadFromJsonAsync<MpOrderResponse>(JsonOptions, cancellationToken)
                   ?? throw new InvalidOperationException($"Resposta vazia ao consultar order {orderId}.");
        }

        private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            if (response.IsSuccessStatusCode) return;

            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException(
                $"Mercado Pago retornou {(int)response.StatusCode}: {body}", null, response.StatusCode);
        }
    }
}