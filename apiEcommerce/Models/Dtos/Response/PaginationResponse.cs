namespace apiEcommerce.Models.Dtos.Response
{
    public class PaginationResponse<T> // Generic class to represent a paginated response
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }

        public ICollection<T> Items { get; set; } = new List<T>(); // Initialize the Items collection to avoid null reference issues
    }
}
