using MyFinances.WebApi.Models.Pagination;
using MyFinances.WebApi.Models;
using MyFinances.WebApi.Models.DTOs;
using System.Threading.Tasks;

namespace MyFinances.WebApi.Repository.Interfaces
{
    public interface ITransactions : IMFService<Transaction>
    {
        Task<PagedList<Transaction>> GetByUserIdAsync(TransactionPayloadDTO transactionPayload, PaginationPayloadDTO parameters);
        Task<PagedList<Transaction>> FilterAsync(TransactionPayloadDTO transactionPayload, PaginationPayloadDTO parameters);
        Task<PagedList<Transaction>> GetByTypeAsync(TransactionPayloadDTO transactionPayload, PaginationPayloadDTO parameters);        
    }
}
