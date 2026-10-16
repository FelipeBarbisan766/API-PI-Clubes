using System.Text.Json.Serialization;

namespace API_PI_Clubes.Application.Interfaces.IServices
{
    public interface IMercadoPagoOrdersClient
    {
        Task<MpOrderResponse> CreateAsync(MpCreateOrderRequest request, string idempotencyKey,
            CancellationToken cancellationToken);

        Task<MpOrderResponse> GetAsync(string orderId, CancellationToken cancellationToken);
    }

    // ---------- Request (POST /v1/orders, Checkout Pro) ----------

    public sealed class MpCreateOrderRequest
    {
        [JsonPropertyName("type")] public string Type { get; init; } = "online";
        [JsonPropertyName("processing_mode")] public string ProcessingMode { get; init; } = "manual";
        [JsonPropertyName("total_amount")] public required string TotalAmount { get; init; }
        [JsonPropertyName("external_reference")] public required string ExternalReference { get; init; }
        [JsonPropertyName("items")] public required List<MpOrderItem> Items { get; init; }
        [JsonPropertyName("config")] public MpOrderConfig? Config { get; init; }
    }

    public sealed class MpOrderItem
    {
        [JsonPropertyName("title")] public required string Title { get; init; }
        [JsonPropertyName("unit_price")] public required string UnitPrice { get; init; }
        [JsonPropertyName("quantity")] public int Quantity { get; init; } = 1;
    }

    public sealed class MpOrderConfig
    {
        [JsonPropertyName("online")] public MpOnlineConfig? Online { get; init; }
    }

    public sealed class MpOnlineConfig
    {
        [JsonPropertyName("success_url")] public string? SuccessUrl { get; init; }
        [JsonPropertyName("failure_url")] public string? FailureUrl { get; init; }
        [JsonPropertyName("pending_url")] public string? PendingUrl { get; init; }
        [JsonPropertyName("auto_return")] public string? AutoReturn { get; init; }
    }

    // ---------- Response ----------

    public sealed class MpOrderResponse
    {
        [JsonPropertyName("id")] public string? Id { get; set; }
        [JsonPropertyName("status")] public string? Status { get; set; }
        [JsonPropertyName("status_detail")] public string? StatusDetail { get; set; }
        [JsonPropertyName("external_reference")] public string? ExternalReference { get; set; }
        [JsonPropertyName("checkout_url")] public string? CheckoutUrl { get; set; }

        // ATENÇÃO: a estrutura de transactions.payments no GET do Checkout Pro eu não consegui
        // confirmar na doc. Confira com um GET real e ajuste. Tudo aqui é opcional de propósito.
        [JsonPropertyName("transactions")] public MpTransactions? Transactions { get; set; }
    }

    public sealed class MpTransactions
    {
        [JsonPropertyName("payments")] public List<MpPayment>? Payments { get; set; }
    }

    public sealed class MpPayment
    {
        [JsonPropertyName("id")] public string? Id { get; set; }
        [JsonPropertyName("status")] public string? Status { get; set; }
        [JsonPropertyName("payment_method")] public MpPaymentMethod? PaymentMethod { get; set; }
    }

    public sealed class MpPaymentMethod
    {
        [JsonPropertyName("id")] public string? Id { get; set; }
        [JsonPropertyName("type")] public string? Type { get; set; }
    }
}