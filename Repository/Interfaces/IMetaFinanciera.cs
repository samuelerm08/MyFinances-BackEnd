using SistemaFinanciero.WebApi.Models;
using SistemaFinanciero.WebApi.Models.DTOs;
using SistemaFinanciero.WebApi.Models.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaFinanciero.WebApi.Repository.Interfaces
{
    public interface IMetaFinanciera
    {
        Task<MetaFinanciera> AltaAsync(MetaFinanciera metaFinanciera);
        Task<MetaFinanciera> ModificarAsync(int id, MetaFinanciera metaFinanciera);
        Task<PagedList<MetaFinanciera>> MetasPorUserId(MetaFinancieraPayload metaFinancieraPayload, PaginationPayloadDTO paginationPayload);
        Task<PagedList<MetaFinanciera>> MetasPorEstado(MetaFinancieraPayload metaFinancieraPayload, PaginationPayloadDTO paginationPayload);
        Task<MetaFinanciera> AgregarMonto(MetaPayload payload);
        Task<MetaFinanciera> RetirarMontoMeta(int metaId);
    }
}
