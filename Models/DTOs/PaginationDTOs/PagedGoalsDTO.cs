using System.Collections.Generic;

namespace SistemaFinanciero.WebApi.Models.DTOs.PaginationDTOs
{
    public class PagedGoalsDTO
    {
        public MetadataDTO Meta { get; set; }
        public List<MetaFinancieraDTO> Data { get; set; }
    }
}
