using System.Collections.Generic;

namespace MyFinances.WebApi.Models.DTOs.PaginationDTOs
{
    public class PagedGoalsDTO
    {
        public MetadataDTO Meta { get; set; }
        public List<FinancialGoalDTO> Data { get; set; }
    }
}
