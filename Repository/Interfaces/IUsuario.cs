using SistemaFinanciero.WebApi.Models;
using SistemaFinanciero.WebApi.Models.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaFinanciero.WebApi.Repository.Interfaces
{
    public interface IUsuario
    {
        Task<UsuarioToken> Login(UsuarioAuth usuario);
        Task<Usuario> Registro(Usuario usuario);
        Task<IEnumerable<Usuario>> ObtenerTodoAsync();
        Task<Usuario> ObtenerPorIdAsync(int id);
        Task<Usuario> ModificarAsync(Usuario usuario);
        Task<Usuario> BajaAsync(int id);
    }
}
