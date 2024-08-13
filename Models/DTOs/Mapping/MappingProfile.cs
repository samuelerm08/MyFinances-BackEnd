using AutoMapper;
using MyFinances.WebApi.Models.DTOs.PaginationDTOs;
using MyFinances.WebApi.Models.Pagination;

namespace MyFinances.WebApi.Models.DTOs.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>();
            CreateMap<Transaction, TransactionDTO>();
            CreateMap<FinancialGoal, FinancialGoalDTO>();
            CreateMap<Balance, BalanceDTO>();
            CreateMap<PagedList<Transaction>, PagedTransactionsDTO>();
            CreateMap<PagedList<FinancialGoal>, PagedGoalsDTO>();
            CreateMap<Metadata, MetadataDTO>();
        }
    }
}
