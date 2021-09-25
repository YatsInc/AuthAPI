using System.ComponentModel.DataAnnotations;

namespace AuthAPI.Models
{
    public class AuthenticateRequest
    {
        [Required]
        public string IdToken { get; set; }
    }
}
