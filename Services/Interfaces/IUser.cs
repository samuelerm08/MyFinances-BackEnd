using MyFinances.WebApi.Models.Security;
using MyFinances.WebApi.Models;
using System.Threading.Tasks;

namespace MyFinances.WebApi.Repository.Interfaces
{
    public interface IUser : IMFService<User>
    {
        Task<UserToken> Login(UserAuth auth);
    }
}
