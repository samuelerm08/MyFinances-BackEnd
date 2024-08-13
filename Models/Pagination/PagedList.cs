using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinances.WebApi.Models.Pagination
{
    public class Metadata
    {
        public Metadata(int page, int pageSize, int totalCount)
        {
            Page = page;
            PageSize = pageSize;
            TotalCount = totalCount;
        }

        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public bool HasNextPage => Page * PageSize < TotalCount;
    }
    public class PagedList<T>
    {
        private PagedList(List<T> data, Metadata meta)
        {
            Data = data;
            Meta = meta;
        }

        public Metadata Meta { get; set; }
        public List<T> Data { get; set; }

        public static async Task<PagedList<T>> Paginate(IQueryable<T> query, int page, int pageSize)
        {
            int totalCount = await query.CountAsync();
            List<T> data = await query.Skip((page - 1) * pageSize)
                                      .Take(pageSize)
                                      .ToListAsync();

            return new PagedList<T>(data, new Metadata(page, pageSize, totalCount));
        }
    }
}
