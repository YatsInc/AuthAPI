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
    }
}
