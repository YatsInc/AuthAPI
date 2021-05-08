using LocalAndExternalAuthAPI.Models;
using System.Threading.Tasks;

namespace LocalAndExternalAuthAPI.IServices
{
    public interface ITokenService
    {
        Task<AuthenticationModel> GetTokenAsync(TokenRequestModel model);
    }
}
