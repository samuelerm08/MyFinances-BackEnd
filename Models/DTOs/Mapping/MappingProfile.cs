using AutoMapper;
using SistemaFinanciero.WebApi.Models.DTOs.PaginationDTOs;
using SistemaFinanciero.WebApi.Models.Pagination;

namespace SistemaFinanciero.WebApi.Models.DTOs.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Usuario, UsuarioDTO>();
            CreateMap<Transaccion, TransaccionDTO>();
            CreateMap<MetaFinanciera, MetaFinancieraDTO>();
            CreateMap<Balance, BalanceDTO>();
            CreateMap<PagedList<Transaccion>, PagedTransactionsDTO>();
            CreateMap<PagedList<MetaFinanciera>, PagedGoalsDTO>();
            CreateMap<Metadata, MetadataDTO>();
        }
    }
}
