using LocalAndExternalAuthAPI.Models;
using System.Threading.Tasks;

namespace LocalAndExternalAuthAPI.IServices
{
    public interface IUserService
    {
        Task<string> RegisterAsync(RegisterModel model);

        Task<string> LoginAsync(LoginModel model);
    }
}
