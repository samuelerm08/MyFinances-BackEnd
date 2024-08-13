using MyFinances.WebApi.Models;
using System.Threading.Tasks;

namespace MyFinances.WebApi.Repository.Interfaces
{
    public interface IBalance
    {
        Task<Balance> ActualBalance(Transaction transaction, bool isLower = false);
        Task<Balance> GoalBalance(
                        FinancialGoal goall, 
                        double? currentAmount = null, 
                        bool isModified = false, 
                        bool isLower = false, 
                        bool isDeleted = false);
        Task<Balance> GetByUserId(int id);
    }
}
