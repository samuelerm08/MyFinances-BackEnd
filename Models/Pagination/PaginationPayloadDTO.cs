namespace MyFinances.WebApi.Models.Pagination
{
    public class PaginationPayloadDTO
    {
        public int PageSize { get; set; }
        public int Page { get; set; } = 1;
    }
}
