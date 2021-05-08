using LocalAndExternalAuthAPI.IServices;
using LocalAndExternalAuthAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LocalAndExternalAuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService tokenService;

        public TokenController(ITokenService tokenService)
        {
            this.tokenService = tokenService;
        }

        [HttpPost]
        public async Task<IActionResult> GetTokenAsync(TokenRequestModel model)
        {
            var result = await tokenService.GetTokenAsync(model);
            return Ok(result);
        }
    }
}
