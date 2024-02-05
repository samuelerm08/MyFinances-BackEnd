using System.Collections.Generic;

namespace SistemaFinanciero.WebApi.Models.DTOs.PaginationDTOs
{
    public class PagedTransactionsDTO
    {
        public MetadataDTO Meta { get; set; }
        public List<TransaccionDTO> Data { get; set; }
    }
}
