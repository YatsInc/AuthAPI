using AuthAPI.Models;
using System.Threading.Tasks;

namespace AuthAPI.IServices
{
    public interface IUserService
    {
        Task<string> RegisterAsync(RegisterModel model);

        Task<string> LoginAsync(LoginModel model);
    }
}
