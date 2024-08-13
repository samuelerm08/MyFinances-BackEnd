using System.Collections.Generic;

namespace MyFinances.WebApi.Models.DTOs.PaginationDTOs
{
    public class PagedTransactionsDTO
    {
        public MetadataDTO Meta { get; set; }
        public List<TransactionDTO> Data { get; set; }
    }
}
