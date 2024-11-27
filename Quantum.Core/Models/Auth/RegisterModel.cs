using System.ComponentModel.DataAnnotations;

namespace Quantum.Core.Models.Auth
{
    public class RegisterModel
    {
        [Required]
        [MinLength(4), MaxLength(25)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }

    }
}
