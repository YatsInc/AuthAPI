using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class SecuredController : ControllerBase
{
    [HttpGet("GetSecuredData")]
    public IActionResult GetSecuredData() =>
        Ok("This Secured Data is available only for Authenticated Users.");

    [HttpPost("PostSecuredData")]
    public IActionResult PostSecuredData() =>
        Ok("This Secured Data is available only for Authenticated Users.");
}
