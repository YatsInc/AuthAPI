using Microsoft.AspNetCore.Identity;

namespace Auth.API.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
