using AuthAPI.IServices;
using AuthAPI.Models;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly JwtGenerator jwtGenerator;

        public UserController(IUserService userService, IConfiguration configuration)
        {
            this.userService = userService;
            this.jwtGenerator = new JwtGenerator(configuration.GetValue<string>("JwtPrivateSigningKey"));
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

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] AuthenticateRequest data)
        {
            GoogleJsonWebSignature.ValidationSettings settings = new GoogleJsonWebSignature.ValidationSettings();

            settings.Audience = new List<string>() { "yourkeyhere" };

            GoogleJsonWebSignature.Payload payload = GoogleJsonWebSignature.ValidateAsync(data.IdToken, settings).Result;

            return Ok(new { AuthToken = jwtGenerator.CreateUserAuthToken(payload.Email) });
        }
    }
}
