using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaFinanciero.WebApi.Repository.Interfaces
{
    public interface IMFService<TEntity> where TEntity : class
    {
        //Interfaz encargada de realizar operaciones CRUD genéricas
        Task<IEnumerable<TEntity>> ObtenerTodoAsync();
        Task<TEntity> ObtenerPorIdAsync(int id);
        Task<TEntity> AltaAsync(TEntity entidad);
        Task<TEntity> ModificarAsync(int id, TEntity entidad);
        Task<TEntity> BajaAsync(int id);
    }
}
