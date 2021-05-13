using LocalAndExternalAuthAPI.IServices;
using LocalAndExternalAuthAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LocalAndExternalAuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterModel model)
        {
            var result = await userService.RegisterAsync(model);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginModel model)
        {
            var result = await userService.LoginAsync(model);

            return Ok(result);
        }
    }
}
