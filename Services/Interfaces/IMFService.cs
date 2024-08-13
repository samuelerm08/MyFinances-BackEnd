using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFinances.WebApi.Repository.Interfaces
{
    public interface IMFService<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(int? id);
        Task<TEntity> CreateAsync(TEntity entity);
        Task<TEntity> ModifyAsync(int? id, TEntity entity);
        Task<TEntity> DeleteAsync(int id);
    }
}
