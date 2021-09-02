using AuthAPI.Models;
using System.Threading.Tasks;

namespace AuthAPI.IServices
{
    public interface ITokenService
    {
        Task<string> GetTokenAsync(ApplicationUser user);
    }
}
