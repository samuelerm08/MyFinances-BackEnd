using SistemaFinanciero.WebApi.Models;
using SistemaFinanciero.WebApi.Models.DTOs;
using SistemaFinanciero.WebApi.Models.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaFinanciero.WebApi.Repository.Interfaces
{
    public interface ITransacciones : IMFService<Transaccion>
    {
        Task<PagedList<Transaccion>> GetTransaccionesUserId(TransaccionPayloadDTO transaccionPayload, PaginationPayloadDTO parameters);
        Task<PagedList<Transaccion>> GetFilteredTransactions(TransaccionPayloadDTO transaccionPayload, PaginationPayloadDTO parameters);
        Task<PagedList<Transaccion>> GetTransaccionesTipo(TransaccionPayloadDTO transaccionPayload, PaginationPayloadDTO parameters);        
    }
}
