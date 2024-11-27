using System.ComponentModel.DataAnnotations;

namespace Quantum.Core.Models.Auth
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public bool KeepMeLoggedIn { get; set; }
    }
}
