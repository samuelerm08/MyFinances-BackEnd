using SistemaFinanciero.WebApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaFinanciero.WebApi.Repository.Interfaces
{
    public interface IBalance
    {
        Task<Balance> BalanceActual(Transaccion transaccion, bool isLower = false);
        Task<Balance> BalanceMeta(
                        MetaFinanciera meta, 
                        double? montoActual = null, 
                        bool isModified = false, 
                        bool isLower = false, 
                        bool isDeleted = false);
        Task<Balance> GetBalanceByUserId(int id);
    }
}
