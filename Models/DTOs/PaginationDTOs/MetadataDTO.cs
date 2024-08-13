namespace MyFinances.WebApi.Models.DTOs.PaginationDTOs
{
    public class MetadataDTO
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public bool HasNextPage => Page * PageSize < TotalCount;
    }
}
