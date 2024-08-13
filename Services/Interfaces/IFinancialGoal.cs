using MyFinances.WebApi.Models.Pagination;
using MyFinances.WebApi.Models;
using MyFinances.WebApi.Models.DTOs;
using System.Threading.Tasks;

namespace MyFinances.WebApi.Repository.Interfaces
{
    public interface IFinancialGoal
    {
        Task<FinancialGoal> CreateAsync(FinancialGoal financialGoal);
        Task<FinancialGoal> ModifyAsync(int id, FinancialGoal financialGoal);
        Task<FinancialGoal> DeleteAsync(int id);
        Task<PagedList<FinancialGoal>> GoalsByUserId(FinancialGoalPayload financialGoalPayload, PaginationPayloadDTO paginationPayload);
        Task<PagedList<FinancialGoal>> GoalsByState(FinancialGoalPayload financialGoalPayload, PaginationPayloadDTO paginationPayload);
        Task<FinancialGoal> Withdraw(int financialGoalId);
    }
}
