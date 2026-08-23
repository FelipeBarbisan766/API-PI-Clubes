namespace API_PI_Clubes.Application.DTOs
{
    public class ErrorResponseDTO
    {
        public int StatusCode { get; set; }
        public string ErrorCode { get; set; } = default!;
        public string Message { get; set; } = default!;
        public IDictionary<string, string[]>? Errors { get; set; }
        public string TraceId { get; set; } = default!;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}