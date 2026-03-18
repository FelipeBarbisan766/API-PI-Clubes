namespace API_PI_Clubes.Shared.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }

        public static ApiResponse<T> Ok(T data)
            => new() { Success = true, Data = data };

        public static ApiResponse<T> Fail(string message)
            => new() { Success = false, Message = message };
    }
}
