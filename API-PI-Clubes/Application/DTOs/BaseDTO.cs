namespace API_PI_Clubes.Application.DTOs
{
    public class ResponseIdDTO
    {
        public Guid Id { get; set; }
    }

    public class PagedResultDTO<T> 
    {
        public IEnumerable<T> Data { get; set; } = [];
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
